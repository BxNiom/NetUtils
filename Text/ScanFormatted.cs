namespace BxNiom.Text;

public static class ScanFormatted {
    private static readonly TypeParser[] TypeParsers = new TypeParser[] {
        new(Types.Character, ParseCharacter),
        new(Types.Decimal, ParseDecimal),
        new(Types.Float, ParseFloat),
        new(Types.Hexadecimal, ParseHexadecimal),
        new(Types.Octal, ParseOctal),
        new(Types.ScanSet, ParseScanSet),
        new(Types.String, ParseString),
        new(Types.Unsigned, ParseDecimal)
    };

    private static readonly List<object> Results = new();

    public static object[] ScanF(this string s, string format) {
        Parse(s, format);
        return Results.ToArray();
    }

    private static int Parse(string input, string format) {
        var inp   = new TextParser(input);
        var fmt   = new TextParser(format);
        var spec  = new FormatSpecifier();
        var count = 0;

        Results.Clear();

        while (!fmt.EndOfText && !inp.EndOfText) {
            if (ParseFormatSpecifier(fmt, spec)) {
                var parser = TypeParsers.First(tp => tp.Type == spec.Type);
                if (parser.Parser(inp, spec)) {
                    count++;
                } else {
                    break;
                }
            } else if (char.IsWhiteSpace(fmt.Peek())) {
                inp.MovePastWhitespace();
                fmt.MoveAhead();
            } else if (fmt.Peek() == inp.Peek()) {
                inp.MoveAhead();
                fmt.MoveAhead();
            } else {
                break;
            }
        }

        return count;
    }

    private static bool ParseFormatSpecifier(TextParser format, FormatSpecifier spec) {
        if (format.Peek() != '%') {
            return false;
        }

        format.MoveAhead();

        if (format.Peek() == '%') {
            return false;
        }

        if (format.Peek() == '*') {
            spec.NoResult = true;
            format.MoveAhead();
        } else {
            spec.NoResult = false;
        }

        var start = format.Position;
        while (char.IsDigit(format.Peek())) {
            format.MoveAhead();
        }

        spec.Width = format.Position > start ? int.Parse(format.Extract(start, format.Position)) : 0;

        if (format.Peek() == 'h') {
            format.MoveAhead();
            if (format.Peek() == 'h') {
                format.MoveAhead();
                spec.Modifier = Modifiers.ShortShort;
            } else {
                spec.Modifier = Modifiers.Short;
            }
        } else if (char.ToLower(format.Peek()) == 'l') {
            format.MoveAhead();
            if (format.Peek() == 'l') {
                format.MoveAhead();
                spec.Modifier = Modifiers.LongLong;
            } else {
                spec.Modifier = Modifiers.Long;
            }
        } else {
            spec.Modifier = Modifiers.None;
        }

        switch (format.Peek()) {
            case 'c':
                spec.Type = Types.Character;
                break;
            case 'd':
            case 'i':
                spec.Type = Types.Decimal;
                break;
            case 'a':
            case 'A':
            case 'e':
            case 'E':
            case 'f':
            case 'F':
            case 'g':
            case 'G':
                spec.Type = Types.Float;
                break;
            case 'o':
                spec.Type = Types.Octal;
                break;
            case 's':
                spec.Type = Types.String;
                break;
            case 'u':
                spec.Type = Types.Unsigned;
                break;
            case 'x':
            case 'X':
                spec.Type = Types.Hexadecimal;
                break;
            case '[':
                spec.Type = Types.ScanSet;
                format.MoveAhead();
                if (format.Peek() == '^') {
                    spec.ScanSetExclude = true;
                    format.MoveAhead();
                } else {
                    spec.ScanSetExclude = false;
                }

                start = format.Position;
                if (format.Peek() == ']') {
                    format.MoveAhead();
                }

                format.MoveTo(']');
                if (format.EndOfText) {
                    throw new Exception("Type specifier expected character : ']'");
                }

                spec.ScanSet = format.Extract(start, format.Position);
                break;
            default:
                var msg = $"Unknown format type specified : '{format.Peek()}'";
                throw new Exception(msg);
        }

        format.MoveAhead();
        return true;
    }

    private static bool ParseCharacter(TextParser input, FormatSpecifier spec) {
        var start = input.Position;
        var count = spec.Width > 1 ? spec.Width : 1;
        while (!input.EndOfText && count-- > 0) {
            input.MoveAhead();
        }

        if (count <= 0 && input.Position > start) {
            if (!spec.NoResult) {
                var token = input.Extract(start, input.Position);
                if (token.Length > 1) {
                    Results.Add(token.ToCharArray());
                } else {
                    Results.Add(token[0]);
                }
            }

            return true;
        }

        return false;
    }

    private static bool ParseDecimal(TextParser input, FormatSpecifier spec) {
        var radix = 10;

        input.MovePastWhitespace();

        var start = input.Position;
        if (input.Peek() == '+' || input.Peek() == '-') {
            input.MoveAhead();
        } else if (input.Peek() == '0') {
            if (char.ToLower(input.Peek(1)) == 'x') {
                radix = 16;
                input.MoveAhead(2);
            } else {
                radix = 8;
                input.MoveAhead();
            }
        }

        while (IsValidDigit(input.Peek(), radix)) {
            input.MoveAhead();
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }

        if (input.Position <= start) {
            return false;
        }

        if (spec.NoResult) {
            return true;
        }

        if (spec.Type == Types.Decimal) {
            AddSigned(input.Extract(start, input.Position), spec.Modifier, radix);
        } else {
            AddUnsigned(input.Extract(start, input.Position), spec.Modifier, radix);
        }

        return true;
    }

    private static bool ParseFloat(TextParser input, FormatSpecifier spec) {
        input.MovePastWhitespace();

        var start = input.Position;
        if (input.Peek() == '+' || input.Peek() == '-') {
            input.MoveAhead();
        }

        var hasPoint = false;
        while (char.IsDigit(input.Peek()) || input.Peek() == '.') {
            if (input.Peek() == '.') {
                if (hasPoint) {
                    break;
                }

                hasPoint = true;
            }

            input.MoveAhead();
        }

        if (char.ToLower(input.Peek()) == 'e') {
            input.MoveAhead();
            if (input.Peek() == '+' || input.Peek() == '-') {
                input.MoveAhead();
            }

            while (char.IsDigit(input.Peek())) {
                input.MoveAhead();
            }
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }


        if (input.Position <= start ||
            !double.TryParse(input.Extract(start, input.Position), out var result)) {
            return false;
        }

        if (spec.NoResult) {
            return true;
        }

        if (spec.Modifier == Modifiers.Long ||
            spec.Modifier == Modifiers.LongLong) {
            Results.Add(result);
        } else {
            Results.Add((float)result);
        }

        return true;
    }

    private static bool ParseHexadecimal(TextParser input, FormatSpecifier spec) {
        input.MovePastWhitespace();

        var start = input.Position;
        if (input.Peek() == '0' && input.Peek(1) == 'x') {
            input.MoveAhead(2);
        }

        while (IsValidDigit(input.Peek(), 16)) {
            input.MoveAhead();
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }

        if (input.Position <= start) {
            return false;
        }

        if (!spec.NoResult) {
            AddUnsigned(input.Extract(start, input.Position), spec.Modifier, 16);
        }

        return true;
    }

    private static bool ParseOctal(TextParser input, FormatSpecifier spec) {
        input.MovePastWhitespace();

        var start = input.Position;
        while (IsValidDigit(input.Peek(), 8)) {
            input.MoveAhead();
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }

        if (input.Position <= start) {
            return false;
        }

        if (!spec.NoResult) {
            AddUnsigned(input.Extract(start, input.Position), spec.Modifier, 8);
        }

        return true;
    }

    private static bool ParseScanSet(TextParser input, FormatSpecifier spec) {
        var start = input.Position;
        if (!spec.ScanSetExclude) {
            while (spec.ScanSet.Contains(input.Peek())) {
                input.MoveAhead();
            }
        } else {
            while (!input.EndOfText && !spec.ScanSet.Contains(input.Peek())) {
                input.MoveAhead();
            }
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }

        if (input.Position > start) {
            if (!spec.NoResult) {
                Results.Add(input.Extract(start, input.Position));
            }

            return true;
        }

        return false;
    }

    private static bool ParseString(TextParser input, FormatSpecifier spec) {
        input.MovePastWhitespace();

        var start = input.Position;
        while (!input.EndOfText && !char.IsWhiteSpace(input.Peek())) {
            input.MoveAhead();
        }

        if (spec.Width > 0) {
            var count = input.Position - start;
            if (spec.Width < count) {
                input.MoveAhead(spec.Width - count);
            }
        }

        if (input.Position > start) {
            if (!spec.NoResult) {
                Results.Add(input.Extract(start, input.Position));
            }

            return true;
        }

        return false;
    }

    private static bool IsValidDigit(char c, int radix) {
        var i = "0123456789abcdef".IndexOf(char.ToLower(c));
        return i >= 0 && i < radix;
    }

    private static void AddSigned(string token, Modifiers mod, int radix) {
        object obj;
        if (mod == Modifiers.ShortShort) {
            obj = Convert.ToSByte(token, radix);
        } else if (mod == Modifiers.Short) {
            obj = Convert.ToInt16(token, radix);
        } else if (mod is Modifiers.Long or Modifiers.LongLong) {
            obj = Convert.ToInt64(token, radix);
        } else {
            obj = Convert.ToInt32(token, radix);
        }

        Results.Add(obj);
    }

    private static void AddUnsigned(string token, Modifiers mod, int radix) {
        object obj;
        switch (mod) {
            case Modifiers.ShortShort:
                obj = Convert.ToByte(token, radix);
                break;
            case Modifiers.Short:
                obj = Convert.ToUInt16(token, radix);
                break;
            case Modifiers.Long:
            case Modifiers.LongLong:
                obj = Convert.ToUInt64(token, radix);
                break;
            default:
                obj = Convert.ToUInt32(token, radix);
                break;
        }

        Results.Add(obj);
    }

    private enum Types {
        Character,
        Decimal,
        Float,
        Hexadecimal,
        Octal,
        ScanSet,
        String,
        Unsigned
    }

    private enum Modifiers {
        None,
        ShortShort,
        Short,
        Long,
        LongLong
    }

    private delegate bool ParseValue(TextParser input, FormatSpecifier spec);

    private struct TypeParser {
        public Types      Type   { get; }
        public ParseValue Parser { get; }

        public TypeParser(Types type, ParseValue parser) {
            if (Parser == null)
                throw new ArgumentNullException(nameof(parser));

            Type   = type;
            Parser = parser;
        }
    }

    private class FormatSpecifier {
        public Types     Type           { get; set; }
        public Modifiers Modifier       { get; set; }
        public int       Width          { get; set; }
        public bool      NoResult       { get; set; }
        public string    ScanSet        { get; set; } = string.Empty;
        public bool      ScanSetExclude { get; set; }
    }
}