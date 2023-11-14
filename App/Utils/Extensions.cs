using System;
using Avalonia.Media;

namespace App.Utils;

public static class Extensions {
    /// <summary>
    /// Конвертирует HEX литерал в цвет
    /// </summary>
    /// <param name="hex">
    /// HEX или uint, байты которого соответствуют цветам, то есть 0x TT RR GG BB
    /// где TT непрозрачность, может быть опущена
    /// RR, GG, BB цвета красный, зеленый, синий соответственно
    /// </param>
    /// <returns></returns>
    public static Color ToColor(this uint hex) {
        var bytes = BitConverter.GetBytes(hex);
        if (bytes.Length == 3) {
            return Color.FromArgb(0xFF, bytes[0], bytes[1], bytes[2]);
        }
        
        return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
    }
}