<?php
// toolset for PlayStation One games
// - little-endian byte order
// - 32-bit negative int
// - 0x200000 RAM
//   - 80000000-8000ffff BIOS
//   - 80010000-801fffef WRAM
//   - 801ffff0-801fffff stack
//   - JUMP   are absolute RAM offset * 4
//   - BRANCH are relative RAM offset * 4
// - fileseek in sectors
//   - n * sector_size
//     - absolute position within the CD
//     - relative position within the file
//   - minute:second:frame (absolute position)
// - Sprites are PsuedoColor
// - Backgrounds are PsuedoColor, very rarely DirectColor
// - RGB555 color format , in ABBBBBGGGGGRRRRR order
// - Alpha is by Additive Blending , as in FG + BG
// - Layers are Top-to-Bottom order
// - 0,0 is at Top-Left corner
// - load/store registers
define("ZERO", chr(  0));
define("BYTE", chr(255));
define("BIT4",  0x0f);
define("BIT8",  0xff);
define("BIT16", 0xffff);
define("BIT24", 0xffffff);
define("BIT32", 0xffffffff);
define("KILO", 1 << 10);
define("MEGA", 1 << 20);
define("GIGA", 1 << 30);
//if ( defined("DRY_RUN") ) // per script

// PHP_VERSION_ID >= 050207
if ( ! defined("PHP_VERSION_ID") )
	trigger_error("requires PHP 5.2.7 or above", E_USER_ERROR);
if ( PHP_VERSION_ID < 050600 )
	trigger_error("PHP 5.5 and below is not supported", E_USER_WARNING);
//////////////////////////////
function str2int( &$str, $pos, $byte, $sign=false )
{
	$sub = substr($str, $pos, $byte);
	if ( $sign )
	{
		$sint = array("", "sint8", "sint16", "sint24", "sint32");
		$func = $sint[$byte];
		return $func($sub);
	}
	return ordint($sub);
}

function substr0( &$str, $pos, $end="" )
{
	if ( empty($end) )  $end = ZERO;
	$len = 0;
	while (1)
	{
		if ( ! isset( $str[$pos+$len] ) )
			break;
		if ( strpos($end, $str[$pos+$len]) !== false )
			break;
		$len++;
	}
	return substr($str, $pos, $len);
}

function ordint( $str )
{
	if ( (int)$str === $str ) // already $int
		return $str;
	$len = strlen($str);
	$int = 0;
	for ( $i=0; $i < $len; $i++ )
	{
		$b = ord( $str[$i] );
		$int += ($b << ($i*8));
	}
	return $int;
}

function chrint( $int, $byte = 0 )
{
	if ( (float)$int === $int ) // in case return from round() ceil() floor()
		$int = (int)$int;
	if ( (int)$int !== $int ) // already $str
		return $int;
		
	$str = "";
	for ( $i=0; $i < $byte; $i++ )
	{
		$b = $int & BIT8;
		$str .= chr($b);
		$int >>= 8;
	}
	while ( strlen($str) < $byte )
		$str .= ZERO;
	return $str;
}

function sint8( $s )
{
	$int = ordint($s);
	if ( $int >> 7 )
		return $int - BIT8 - 1;
	return $int;
}

function sint16( $s )
{
	$int = ordint($s);
	if ( $int >> 15 )
		return $int - BIT16 - 1;
	return $int;
}

function sint24( $s )
{
	$int = ordint($s);
	if ( $int >> 23 )
		return $int - BIT24 - 1;
	return $int;
}

function sint32( $s )
{
	$int = ordint($s);
	if ( $int >> 31 )
		return $int - BIT32 - 1;
	return $int;
}

function strupd( &$file, $pos, $str )
{
	$len = strlen($str);
	for ( $i=0; $i < $len; $i++ )
		$file[$pos+$i] = $str[$i];
	return;
}

function int_clamp( $int, $min, $max )
{
	if ( $int < $min )  $int = $min;
	if ( $int > $max )  $int = $max;
	return (int)round($int);
}

function int_ceil( $int, $ceil )
{
	// 3 -> 4 and -3 -> -4
	$int = (int)$int;
	$step = ( $int < 0 ) ? -1 : 1;
	while ( ($int % $ceil) != 0 )
		$int += $step;
	return $int;
}

function int_floor( $int, $floor )
{
	// 3 -> 2 and -3 -> -2
	$int = (int)$int;
	$step = ( $int < 0 ) ? 1 : -1;
	while ( ($int % $floor) != 0 )
		$int += $step;
	return $int;
}

function arrayint( &$array )
{
	foreach ( $tmp as $k => $v )
		$array[$k] = (int)$v;
	return;
}

function arrayhex( &$array )
{
	foreach ( $array as $k => $v )
		$array[$k] = hexdec($v);
	return;
}

function load_file( $fname )
{
	if ( is_file($fname) )
		return file_get_contents($fname);

	// case-insensitive file load
	$fname = strtoupper($fname);
	if ( is_file($fname) )
		return file_get_contents($fname);

	$fname = strtolower($fname);
	if ( is_file($fname) )
		return file_get_contents($fname);

	return "";
}

function load_text( $fname )
{
	$txt = load_file($fname);
	if ( empty($txt) )  return "";

	$txt = str_replace("\r\n", "\n", $txt);
	$txt = str_replace("\r", "\n", $txt);
	return explode("\n", $txt);
}

function save_file( $fname, $str )
{
	if ( defined("DRY_RUN") )
		return;
	if ( empty($str) )
		return;
	if ( strpos($fname, '\\') )
		$fname = str_replace('\\', '/', $fname);

	if ( strpos($fname, '/') )
		@mkdir( dirname($fname), 0755, true );
	file_put_contents($fname, $str);
	return;
}

function req_file( $fname )
{
	if ( ! file_exists($fname) )
		trigger_error("req_file( $fname ) : not found\n", E_USER_ERROR);
	return file_get_contents($fname);
}

function patchfile( $fname )
{
	$fn = __DIR__ . "/patch/$fname";
	if ( ! file_exists($fn) )
		return array();

	$data = array();
	$sect = "";
	foreach ( file($fn) as $v )
	{
		$v = preg_replace("|[\s]+|", '', $v);
		if ( empty($v) )
			continue;
		if ( $v[0] == '#' ) // comments
			continue;
		if ( $v[0] == '[' ) // section header
		{
			$sect = trim($v, '[]');
			continue;
		}

		list($vk,$vv) = explode('=', $v);
		if ( strpos($vv, ',') !== false )
			$vv = explode(',', $vv);
		$data[$sect][$vk] = $vv;
	}
	return $data;
}

function lsfile_r( $dir, &$list)
{
	if ( empty($dir) || ! is_dir($dir) )
		return;
	if ( is_link($dir) ) // skip shortcuts / symbolic links
		return;
	$func = __FUNCTION__;
	foreach ( scandir($dir) as $en )
	{
		if ( $en[0] == '.' )
			continue;
		$fn = "$dir/$en";

		if ( is_file($fn) )  $list[] = $fn;
		else
		if ( is_dir ($fn) )  $func($fn, $list);
	}
	return;
}

function rmfile_r( $dir )
{
	if ( empty($dir) )
		return;
	if ( is_file($dir) )
		return unlink($dir);
	if ( is_dir($dir) )
	{
		$list = array();
		lsfile_r( $dir, $list );
		if ( empty($list) )
			return;
		foreach ( $list as $en )
			unlink($en);
	}
	return;
}

function debug( $str, $sep = 4 )
{
	echo "DEBUG : ";
	$len = strlen($str);
	for ( $i=0; $i < $len; $i++ )
	{
		if ( $i > 0 && ($i % $sep) == 0 )
			echo "  ";

		$b = ord( $str[$i] );
		if ( $b == 0 )
			echo "-- ";
		else
			printf("%02x ", $b);
	}
	echo "\n";
	return;
}

function trace()
{
	if ( ! defined("TRACE") )
		return;
	$args = func_get_args();
	$echo = array_shift($args);
	vprintf($echo, $args);
	return;
}

// E_USER_ERROR
// E_USER_WARNING
// E_USER_NOTICE
// E_USER_DEPRECATED
function zero_watch( $name, $chr )
{
	$err = sprintf("ERROR $name not zero [%2x]\n", ord($chr));
	if ( $chr != ZERO )
		trigger_error($err, E_USER_WARNING);
	return;
}

function byte_watch( $name, $chr )
{
	$err = sprintf("ERROR $name not 0xFF [%2x]\n", ord($chr));
	if ( $chr != BYTE )
		trigger_error($err, E_USER_WARNING);
	return;
}

function neg_warn( $name, $int )
{
	$err = sprintf("ERROR $name is negative [%d]\n", $int);
	if ( $int < 0 )
		trigger_error($err, E_USER_WARNING);
	return;
}

function flag_warn( $name, $int )
{
	$err = sprintf("ERROR $name some flags is set [%x]\n", $int);
	if ( $int != 0 )
		trigger_error($err, E_USER_WARNING);
	return;
}
//////////////////////////////
define("PIX_BLACK", ZERO . ZERO . ZERO . BYTE);
define("PIX_WHITE", BYTE . BYTE . BYTE . BYTE);
define("PIX_ALPHA", ZERO . ZERO . ZERO . ZERO);

function canvpix( $w, $h, $c = PIX_ALPHA )
{
	$s = $w * $h;
	return str_repeat($c, $s);
}

function rgb555( $str )
{
	// 15-bit ABGR
	// RGB = c6         52         52
	//     = 1100 0110  0101 0010  0101 0010
	//     = 11000      01010      01010
	// 15-bit = 0(A) 01010(B) 01010(G) 11000(R)
	//        = 0010 1001 0101 1000 = 2958
	//        = 58 29
	$pal = ordint($str);

	$b = ($pal >> 7) & 0xf8; // <<  1 >> 8 == >> 7
	$g = ($pal >> 2) & 0xf8; // <<  6 >> 8 == >> 2
	$r = ($pal << 3) & 0xf8; // << 11 >> 8 == << 3
	$a = BYTE;

	$str = chr($r) . chr($g) . chr($b) . $a;
	return $str;
}

function pal555( $str )
{
	$clut = "";
	$siz = strlen($str);
	for ( $i=0; $i < $siz; $i += 2 )
		$clut .= rgb555( $str[$i+0] . $str[$i+1] );
	return $clut;
}

function strpal555( &$file, $pos, $cc )
{
	$str = substr($file, $pos, $cc*2);
	return pal555($str);
}

function mstrpal555( &$file, $pos, $cc, $cn )
{
	$ret = array();
	while ( $cn > 0 )
	{
		$ret[] = strpal555($file, $pos, $cc);
		$pos += ($cc*2);
		$cn--;
	}
	return $ret;
}

function grayclut( $cc )
{
	$grad = gradient($cc, PIX_BLACK, PIX_WHITE);
	return implode('', $grad);
}

function gradient( $cc, $src, $dst )
{
	$r1 = ord( $src[0] );
	$g1 = ord( $src[1] );
	$b1 = ord( $src[2] );
	$a1 = ord( $src[3] );
	$r2 = ord( $dst[0] );
	$g2 = ord( $dst[1] );
	$b2 = ord( $dst[2] );
	$a2 = ord( $dst[3] );

	$sr = ($r2 - $r1) / ($cc - 1);
	$sg = ($g2 - $g1) / ($cc - 1);
	$sb = ($b2 - $b1) / ($cc - 1);
	$sa = ($a2 - $a1) / ($cc - 1);

	$clut = array();
	for ( $i=0; $i < $cc; $i++ )
	{
		$clut[] = chr($r1) . chr($g1) . chr($b1) . chr($a1);
		$r1 = int_clamp($r1 + $sr, 0, BIT8);
		$g1 = int_clamp($g1 + $sg, 0, BIT8);
		$b1 = int_clamp($b1 + $sb, 0, BIT8);
		$a1 = int_clamp($a1 + $sa, 0, BIT8);
	}
	return $clut;
}
//////////////////////////////
function COPYPIX_DEF()
{
	// PHP 5.6 fix
	// Warning: Constants may only evaluate to scalar values
	$php56 = array(
		'rgba' => array(
			'w' => 0,
			'h' => 0,
			'pix' => "",
		),
		'src' => array(
			'w' => 0,
			'h' => 0,
			'pix' => "",
			'pal' => "",
		),
		'dx' => 0,
		'dy' => 0,
		'bgzero' => -1, // 1-byte (CLUT,int) or 4-byte (RGBA,string) to ignore
		'vflip'  => false,
		'hflip'  => false,
		'rotate' => array(0,0,0),
		'vertex' => array(),
		'alpha'  => "", // callback
	);
	return $php56;
}
//////////////////////////////
function rippix_errs( $sx, $sy, $w, $h, $sw, $sh )
{
	$err = "";
	if ( $sx < 0 )
		$err .= sprintf("rippix() sx = %d\n", $sx);
	if ( $sy < 0 )
		$err .= sprintf("rippix() sy = %d\n", $sy);
	if ( ($sx + $w) > $sw )
		$err .= sprintf("rippix() sx + w = %d + %d [%d]\n", $sx, $w, $sw);
	if ( ($sy + $h) > $sh )
		$err .= sprintf("rippix() sy + h = %d + %d [%d]\n", $sy, $h, $sh);

	if ( ! empty($err) )
	{
		trigger_error($err, E_USER_ERROR);
		return true;
	}
	return false;
}

function rippix4( &$pix, $sx, $sy, $w, $h, $sw, $sh )
{
	if ( defined("DRY_RUN") )
		return;
	$sx /= 2;
	$w  /= 2;
	$sw /= 2;
	if ( rippix_errs($sx, $sy, $w, $h, $sw, $sh) )
		return;

	$rip = "";
	for ( $y=0; $y < $h; $y++ )
	{
		$syy = ($sy + $y) * $sw;
		for ( $x=0; $x < $w; $x++ )
		{
			$sxx = $syy + $sx + $x;
			$p = ord( $pix[$sxx] );

			$buf = "";
			$buf .= chr( ($p >> 0) & BIT4 );
			$buf .= chr( ($p >> 4) & BIT4 );

			$rip .= $buf;
		} // for ( $x=0; $x < ($w/2); $x++ )
	} // for ( $y=0; $y < $h; $y++ )
	return $rip;
}

function rippix8( &$pix, $sx, $sy, $w, $h, $sw, $sh )
{
	if ( defined("DRY_RUN") )
		return;
	if ( rippix_errs($sx, $sy, $w, $h, $sw, $sh) )
		return;

	$rip = "";
	for ( $y=0; $y < $h; $y++ )
	{
		$sxx = ($sy + $y) * $sw + $sx;
		$rip .= substr($pix, $sxx, $w);
	} // for ( $y=0; $y < $h; $y++ )
	return $rip;
}

function copypix_errs( &$pix )
{
	$err = "";
	if ( $pix['dx'] < 0 )
		$err .= sprintf("copypix() dx = %d\n", $pix['dx']);
	if ( $pix['dy'] < 0 )
		$err .= sprintf("copypix() dy = %d\n", $pix['dy']);
	if ( ($pix['dx'] + $pix['src']['w']) > $pix['rgba']['w'] )
		$err .= sprintf("copypix() dx + sw = %d + %d\n", $pix['dx'], $pix['src']['w']);
	if ( ($pix['dy'] + $pix['src']['h']) > $pix['rgba']['h'] )
		$err .= sprintf("copypix() dy + sh = %d + %d\n", $pix['dy'], $pix['src']['h']);

	if ( ! empty($err) )
	{
		trigger_error($err, E_USER_ERROR);
		return true;
	}
	return false;
}

function copypix( &$pix, $byte=1 )
{
	if ( copypix_errs($pix) )
		return;
	if ( defined("DRY_RUN") )
		return;

	flippix($pix, $byte);

	// src rotation
	//   0x100 == 360 degree == 2 * pi() radian
	//   0x80  == 180 degree == pi() radian
	//   0x01  == 180/128 degree == pi()/128 radian
	//         == 1.40625 degree == 0.02454369260617... radian
	// Legend of Mana
	//   rotation based on dx,dy , parts at +0,+0
	// Xenogears
	//   rotation based on dx,dy , parts at +rx,+ry
	$rot = ($pix['rotate'][0] & BIT8) * pi() / 0x80;
	$sin = sin($rot);
	$cos = cos($rot);
	$hyp = hypot($pix['src']['w'], $pix['src']['h']) * 0.6;

	// 10x10 square (-5,-5,5,5) rotate 45 degree = (  0, -7, 0, 7) or ~14x14 square
	// hypotenuse 10x10 square = 14.1421        += (-14,-21,14,21)
	//   center-based with hypot                 = (-14,-14,14,14) or ~28x28 square
	$tx = $pix['rotate'][1] + ($pix['src']['w'] / 2);
	$ty = $pix['rotate'][2] + ($pix['src']['h'] / 2);
		$cx = $tx * $cos - $ty * $sin;
		$cy = $tx * $sin + $ty * $cos;
	$x1 = (int)($cx - $hyp);
	$y1 = (int)($cy - $hyp);
	$x2 = (int)($cx + $hyp);
	$y2 = (int)($cy + $hyp);
	//printf("x %d y %d w %d h %d hyp %d\n", $x1, $y1, $x2-$x1, $y2-$y1, $hyp);

	// dest rotation + alpha blend + copy
	$rot = ((0x100 - $pix['rotate'][0]) & BIT8) * pi() / 0x80;
	$sin = sin($rot);
	$cos = cos($rot);
	for ( $y=$y1; $y <= $y2; $y++ )
	{
		$dy = $y + $pix['dy'];
		if ( $dy < 0 || $dy >= $pix['rgba']['h'] )
			continue;

		for ( $x=$x1; $x <= $x2; $x++ )
		{
			$dx = $x + $pix['dx'];
			if ( $dx < 0 || $dx >= $pix['rgba']['w'] )
				continue;

			$rx = (int)round($x * $cos - $y * $sin) - $pix['rotate'][1];
			$ry = (int)round($x * $sin + $y * $cos) - $pix['rotate'][2];
			if ( $rx < 0 || $rx >= $pix['src']['w'] )
				continue;
			if ( $ry < 0 || $ry >= $pix['src']['h'] )
				continue;

			$sxx = ($ry * $pix['src']['w']) + $rx;
			$c1 = substr($pix['src']['pix'], $sxx*$byte, $byte);

			// Ignore BG color
			//   byte 1 bgzero 0    = cmp c1, bgzero
			//   byte 1 bgzero RGBA = c1 -> RGBA , cmp c1, bgzero
			//   byte 4 bgzero 0    = skip
			//   byte 4 bgzero RGBA = cmp c1, bgzero
			// Xenogears Lucca = both 0th and 15th color are #000
			//                   ignore 0th , but not #000
			if ( $byte == 1 )
			{
				$c1 = ord($c1);
				if ( $pix['bgzero'] === (int)$pix['bgzero'] )
				{
					if ( $c1 == $pix['bgzero'] )
						continue;
				}
				$c1 = substr($pix['src']['pal'], $c1*4, 4);
			}

			// c1 is now RGBA
			// Yuna Mika copyrgba()
			if ( $c1 === $pix['bgzero'] )
				continue;

			$dxx = ($dy * $pix['rgba']['w']) + $dx;
			$c2 = substr($pix['rgba']['pix'], $dxx*4, 4);

			// ALPHA BLENDING
			// using callbacks due to implementation difference
			// Legend of Mana @ E=M+I
			// Legend of Mana /wm/wmland/* @ 1 is E=M+I @ 3 is E=(M/5)+I
			$alpha = $pix['alpha'];
			if ( function_exists($alpha) )
				$c1 = $alpha($c1, $c2);

			// COPY / UPDATE PIXEL
			strupd($pix['rgba']['pix'], $dxx*4, $c1);

		} // for ( $x=$x1; $x < $x2; $x++ )
	} // for ( $y=$y1; $y < $y2; $y++ )

	return;
}

function trimpix( &$pix )
{
	$x1 = 0;
	$x2 = $pix['rgba']['w'];
	$y1 = 0;
	$y2 = $pix['rgba']['h'];
	$TRIM_SZ = 16;

	// keep it square
	while (1)
	{
		$row = $pix['rgba']['w'] * 4;
		$b = "";

		// trim height
		$p = $y1 * $row;
		$b .= substr($pix['rgba']['pix'], $p, $row*$TRIM_SZ);

		$p = ($y2 - $TRIM_SZ) * $row;
		$b .= substr($pix['rgba']['pix'], $p, $row*$TRIM_SZ);

		// trim width
		for ( $y=$y1; $y < $y2; $y++ )
		{
			$p = ($y * $row) + ($x1 * 4);
			$b .= substr($pix['rgba']['pix'], $p, 4*$TRIM_SZ);
		}
		for ( $y=$y1; $y < $y2; $y++ )
		{
			$p = ($y * $row) + (($x2-$TRIM_SZ) * 4);
			$b .= substr($pix['rgba']['pix'], $p, 4*$TRIM_SZ);
		}

		if ( trim($b, ZERO) != '' )
			break;

		$y1 += $TRIM_SZ;
		$y2 -= $TRIM_SZ;
		$x1 += $TRIM_SZ;
		$x2 -= $TRIM_SZ;
	}
	printf("TRIM %4d , %4d , %4d , %4d\n", $x1, $y1, $x2, $y2);

	$w = $x2 - $x1;
	$h = $y2 - $y1;
	$canv = "";
	for ( $y=$y1; $y < $y2; $y++ )
	{
		$p = $y * $pix['rgba']['w'] + $x1;
		$canv .= substr($pix['rgba']['pix'], $p*4, $w*4);
	}
	$pix['rgba']['w'] = $w;
	$pix['rgba']['h'] = $h;
	$pix['rgba']['pix'] = $canv;
	return;
}

function savpix( $fn, &$pix, $trim = false )
{
	if ( defined("DRY_RUN") )
		return;

	if ( trim($pix['rgba']['pix'], ZERO) == '' )
		return trigger_error("ERROR $fn [pix] blank\n", E_USER_WARNING);
	if ( $trim )
		trimpix($pix);

	$rgba = "RGBA";
	$rgba .= chrint( $pix['rgba']['w'], 4 );
	$rgba .= chrint( $pix['rgba']['h'], 4 );
	$rgba .= $pix['rgba']['pix'];
	save_file("$fn.rgba", $rgba);
	return;
}

function alpha_add( $fg, $bg )
{
	// FG is RGB from palette
	// if BG is blank , FG RGB -> RGBA
	// if BG is solid , FG += BG
	// if BG is RGBA  , BG RGBA -> RGB, FG += BG, FG RGB -> RGBA
	switch ( $bg[3] )
	{
		// blend w/normal pixels
		case BYTE:
			$pix = "";
			for ( $i=0; $i < 4; $i++ )
			{
				$p1 = ord( $fg[$i] );
				$p2 = ord( $bg[$i] );
				$p = int_clamp($p1 + $p2, 0, BIT8);
				$pix .= chr($p);
			}
			return $pix;

		// blend w/transparent pixel
		// #202020 + #00000000 = #ffffff20
		// #204080 + #00000000 = #4080ff80
		case ZERO:
			$r = ord($fg[0]);
			$g = ord($fg[1]);
			$b = ord($fg[2]);

			$a = ( $r > $g ) ? $r : $g;
			$a = ( $b > $a ) ? $b : $a;

			$r = int_clamp($r / $a * BIT8, 0, BIT8);
			$g = int_clamp($g / $a * BIT8, 0, BIT8);
			$b = int_clamp($b / $a * BIT8, 0, BIT8);
			return chr($r) . chr($g) . chr($b) . chr($a);

		// blend w/semi-transparent pixel
		// #202020   + #204080   = #4060a0
		// #ffffff20 + #4080ff80 = #6699ffa0 (NOT #ffffffa0)
		// #202020   + #4080ff80 = #6699ffa0
		default:
			$a2 = ord($bg[3]);
			$r2 = ord($bg[0]) / BIT8 * $a2;
			$g2 = ord($bg[1]) / BIT8 * $a2;
			$b2 = ord($bg[2]) / BIT8 * $a2;

			$r = int_clamp(ord($fg[0]) + $r2, 0, BIT8);
			$g = int_clamp(ord($fg[1]) + $g2, 0, BIT8);
			$b = int_clamp(ord($fg[2]) + $b2, 0, BIT8);
			$nfg = chr($r) . chr($g) . chr($b) . BYTE;

			$func = __FUNCTION__;
			return $func($nfg, PIX_ALPHA);
	}
	return $fg;
}

function flippix( &$pix, $byte=1 )
{
	// horizontal flip + vertical flip
	$flip = str_repeat(ZERO, $pix['src']['w'] * $pix['src']['h'] * $byte);
	for ( $y=0; $y < $pix['src']['h']; $y++ )
	{
		$syy = $y * $pix['src']['w'];
		$dyy = $y;
		if ( $pix['vflip'] )
			$dyy = $pix['src']['h'] - 1 - $y;
		$dyy *= $pix['src']['w'];

		for ( $x=0; $x < $pix['src']['w']; $x++ )
		{
			$sxx = $syy + $x;
			$dxx = $x;
			if ( $pix['hflip'] )
				$dxx = $pix['src']['w'] - 1 - $x;
			$dxx += $dyy;

			$c1 = substr($pix['src']['pix'], $sxx*$byte, $byte);
			strupd($flip, $dxx*$byte, $c1);

		} // for ( $x=0; $x < $pix['src']['w']; $x++ )
	} // for ( $y=0; $y < $pix['src']['h']; $y++ )

	$pix['src']['pix'] = $flip;
	return;
}

function scalepix( &$pix, $fact_x, $fact_y, $byte=1 )
{
	if ( defined("DRY_RUN") )
		return;
	if ( (int)($fact_x*100) === 100 && (int)($fact_y*100) === 100 )
		return;

	$nw = (int)ceil( $pix['src']['w'] * $fact_x );
	$nh = (int)ceil( $pix['src']['h'] * $fact_y );
	$canv = canvpix($nw, $nh);
	$mx = $pix['src']['w'] / $nw;
	$my = $pix['src']['h'] / $nh;
	for ( $y=0; $y < $nh; $y++ )
	{
		$dyy = $y * $nw;
		$syy = (int)($y * $my) * $pix['src']['w'];
		for ( $x=0; $x < $nw; $x++ )
		{
			$dxx = $dyy + $x;
			$sxx = (int)($x * $mx) + $syy;

			$c1 = substr($pix['src']['pix'], $sxx*$byte, $byte);
			strupd($canv, $dxx*$byte, $c1);
		} // for ( $x=0; $x < $nw; $x++ )
	} // for ( $y=0; $y < $nh; $y++ )

	$pix['src']['w'] = $nw;
	$pix['src']['h'] = $nh;
	$pix['src']['pix'] = $canv;
	return;
}
//////////////////////////////
function psx_patch( $dir, $pfx )
{
	foreach( file("$dir/system.cnf") as $sys )
	{
		if ( stripos($sys, 'boot') === false )
			continue;
		preg_match('|[a-z]{4}_[0-9]{3}\.[0-9]{2}|i', $sys, $m);
		if ( $m[0] == '' )
			return array();
		return patchfile("{$pfx}_{$m[0]}.txt");
	}
	return array();
}

function psx_exe( &$psxram, $psxexe )
{
	if ( empty($psxram) )
		return "";

	$bin = load_file($psxexe);
	if ( empty($bin) || substr($bin, 0, 8) != 'PS-X EXE' )
		return;
	$off1 = str2int($bin, 0x10, 3); // mips start()
	$off2 = str2int($bin, 0x18, 3); // mips ram
	$off3 = str2int($bin, 0x1c, 3); // mips size
	$off4 = str2int($bin, 0x30, 3); // init stack

	// Tokimemo 2 EVS series has garbage data at the end
	$bin = substr($bin, 0x800, $off3);
	strupd($psxram, $off2, $bin);
	return;
}

function psxtim( &$file )
{
	$tim = array();
	$pos = 0;

	// TIM header
	$tim['t'] = str2int( $file, $pos+4, 1 );
	$tim['cc'] = 0;
	if ( $tim['t'] == 9 )  $tim['cc'] = 0x100;
	if ( $tim['t'] == 8 )  $tim['cc'] = 0x10;
	if ( $tim['cc'] == 0 )
		return $tim;
	$pos += 8;

	// CLUT + header
	$siz = str2int( $file, $pos+0, 4 ) - 12;
	$tim['cn'] = (int)($siz / ($tim['cc'] * 2));
	$tim['clut'] = mstrpal555($file, $pos+12, $tim['cc'], $tim['cn']);
	$pos += $siz + 12;

	// PIX + header
	$siz = str2int( $file, $pos+0, 4 ) - 12;
	$tim['w'] = str2int($file, $pos+ 8, 2);
	$tim['h'] = str2int($file, $pos+10, 2);
	$pos += 12;

	if ( $tim['t'] == 9 )
	{
		$tim['w'] *= 2;
		$tim['pix'] = substr($file, $pos, $siz);

		printf("psxtim() = %d , %d x %d , %d x %d\n",
			$tim['t'], $tim['w'], $tim['h'], $tim['cc'], $tim['cn']
		);
		return $tim;
	}
	if ( $tim['t'] == 8 )
	{
		$tim['w'] *= 4;
		$tim['pix'] = "";

		while ( $siz > 0 )
		{
			$b = ord( $file[$pos] );
			$b1 = ($b >> 0) & BIT4;
			$b2 = ($b >> 4) & BIT4;
			$tim['pix'] .= chr($b1) . chr($b2);

			$siz--;
			$pos++;
		}

		printf("psxtim() = %d , %d x %d , %d x %d\n",
			$tim['t'], $tim['w'], $tim['h'], $tim['cc'], $tim['cn']
		);
		return $tim;
	}
	return $tim;
}
//////////////////////////////
