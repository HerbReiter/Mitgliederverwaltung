using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace My.Parsing
{
    #region Enums
    public enum ParseResult : int
    {
        Success,
        Empty,
        ParseError,
        EmptyError,
        CheckError
    }
    public enum FormatResult : int
    {
        Success,
        Empty,
        Error
    }
    public enum DateFormat : int
    {
        ShortDate_yyyy,
        ShortDate_yy
    }
    public enum TimeFormat : int
    {
        HH_mm_ss,
        HH_mm
    }
    #endregion

    #region Abstrakte Klassen
    public abstract class ParserBase
    {
        #region Private Felder
        private static CultureInfo mDefaultCulture = CultureInfo.InvariantCulture;
        #endregion

        #region Geschützte Felder
        protected CultureInfo mCulture;
        protected string mPattern = string.Empty;
        protected bool mAllowEmpty;
        #endregion

        #region Öffentliche Eigenschaften
        public string CultureName
        {
            get { return mCulture.Name; }
        }
        public int CultureId
        {
            get { return mCulture.LCID; }
        }
        public string Pattern
        {
            get { return mPattern; }
        }
        public bool AllowEmpty
        {
            get { return mAllowEmpty; }
        }
        #endregion

        #region Geschützte Konstruktoren
        protected ParserBase(string cultureName, bool allowEmty)
        {
            mAllowEmpty = allowEmty;

            try
            {
                if (cultureName != null)
                    mCulture = new CultureInfo(cultureName, false);
                else
                    mCulture = mDefaultCulture;
            }
            catch { mCulture = mDefaultCulture; }
        }
        protected ParserBase(int cultureId, bool allowEmty)
        {
            mAllowEmpty = allowEmty;

            try
            {
                mCulture = new CultureInfo(cultureId, false);
            }
            catch { mCulture = mDefaultCulture; }
        }
        #endregion

        #region Öffentliche virtuelle Methoden
        public virtual FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }
                else
                    entry = string.Format(mCulture, "{0:" + mPattern + "}", value);

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        public virtual string ToString(object value)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                    return string.Empty;
                else
                    return string.Format(mCulture, "{0:" + mPattern + "}", value);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region Abstrakte Methoden
        public abstract ParseResult TryParse(string entry, out object value, out string formatted);
        public abstract ParseResult TryParse(string entry, out object value);
        public abstract ParseResult TryParse(string entry, out string formatted);
        #endregion
    }
    public abstract class NumericParserBase : ParserBase
    {
        #region Geschützte Felder
        protected NumberStyles mStyles;
        protected string mPosSign;
        protected string mNegSign;
        #endregion

        #region Geschützte Konstruktoren
        protected NumericParserBase(string cultureName, bool showThousandsSep,
            byte decimals, byte leadingZeros, bool allowEmpty)
            : base(cultureName, allowEmpty)
        {
            SetParams(showThousandsSep, decimals, leadingZeros);
        }
        protected NumericParserBase(int cultureId, bool showThousandsSep,
            byte decimals, byte leadingZeros, bool allowEmpty)
            : base(cultureId, allowEmpty)
        {
            SetParams(showThousandsSep, decimals, leadingZeros);
        }
        #endregion

        #region Geschützte Methoden
        protected void PrepareEntry(ref string entry)
        {
            if (entry == null)
            {
                entry = string.Empty;
                return;
            }

            entry = entry.Trim();

            if (entry.StartsWith(mNegSign) &&
                entry.Length > mNegSign.Length &&
                entry[mNegSign.Length] == ' ')
                entry = mNegSign + entry.Substring(mNegSign.Length).TrimStart();
            else if (entry.StartsWith(mPosSign) &&
                entry.Length > mPosSign.Length &&
                entry[mPosSign.Length] == ' ')
                entry = mPosSign + entry.Substring(mPosSign.Length).TrimStart();
        }
        #endregion

        #region Private Methoden
        private void SetParams(bool showThousandsSep, byte decimals, byte leadingZeros)
        {
            string temp;

            if (leadingZeros < 2)
                temp = showThousandsSep ? "#,0" : "0";
            else
                temp = showThousandsSep ?
                    ",0".PadLeft(leadingZeros + 1, '0') :
                    "0".PadRight(leadingZeros, '0');

            mPattern = decimals > 0 ?
                temp + ".0".PadRight(decimals + 1, '0') :
                temp;

            mStyles = NumberStyles.Integer;

            if (showThousandsSep)
                mStyles |= NumberStyles.AllowThousands;

            if (decimals > 0)
                mStyles |= NumberStyles.AllowDecimalPoint;

            mPosSign = mCulture.NumberFormat.PositiveSign;
            mNegSign = mCulture.NumberFormat.NegativeSign;
        }
        #endregion
    }
    #endregion

    #region Numerische Parser
    public sealed class ByteParser : NumericParserBase
    {
        #region Private Felder
        private Func<byte, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public Func<byte, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public ByteParser(string cultureName, byte decimals, 
            byte leadingZeros, bool allowEmpty)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty) { }
        public ByteParser(int cultureId, byte decimals, 
            byte leadingZeros, bool allowEmpty)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty) { }
        public ByteParser(string cultureName, byte decimals, 
            byte leadingZeros, bool allowEmpty, Func<byte, bool> checkParsed)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty) 
        {
            mCheckParsed = checkParsed;
        }
        public ByteParser(int cultureId, byte decimals, byte leadingZeros, 
            bool allowEmpty, Func<byte, bool> checkParsed)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out byte value, out string formatted)
        {
            value = 0;
            formatted = string.Empty;

            try
            {
                PrepareEntry(ref entry);

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!byte.TryParse(entry, mStyles, mCulture, out value))
                    return ParseResult.ParseError;

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                value = 0;
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParse(string entry, out byte value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            byte val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            byte val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            byte val;
            return TryParse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToByte(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion
    }
    public sealed class IntegerParser : NumericParserBase
    {
        #region Private Felder
        private Func<int, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public Func<int, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public IntegerParser(string cultureName, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureName, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public IntegerParser(int cultureId, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureId, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public IntegerParser(string cultureName, byte decimals, byte leadingZeros, 
            bool allowEmpty, Func<int, bool> checkParsed)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty) 
        {
            mCheckParsed = checkParsed;
        }
        public IntegerParser(int cultureId, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<int, bool> checkParsed)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out int value, out string formatted)
        {
            value = 0;
            formatted = string.Empty;

            try
            {
                PrepareEntry(ref entry);

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!int.TryParse(entry, mStyles, mCulture, out value))
                    return ParseResult.ParseError;

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                value = 0;
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParse(string entry, out int value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            int val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            int val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            int val;
            return TryParse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToInt32(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion
    }
    public sealed class LongParser : NumericParserBase
    {
        #region Private Felder
        private Func<long, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public Func<long, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public LongParser(string cultureName, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureName, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public LongParser(int cultureId, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureId, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public LongParser(string cultureName, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<long, bool> checkParsed)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        public LongParser(int cultureId, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<long, bool> checkParsed)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out long value, out string formatted)
        {
            value = 0;
            formatted = string.Empty;

            try
            {
                PrepareEntry(ref entry);

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!long.TryParse(entry, mStyles, mCulture, out value))
                    return ParseResult.ParseError;

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                value = 0;
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParse(string entry, out long value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            long val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            long val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            long val;
            return TryParse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToInt64(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion
    }
    public sealed class DoubleParser : NumericParserBase
    {
        #region Private Felder
        private Func<double, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public Func<double, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public DoubleParser(string cultureName, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureName, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public DoubleParser(int cultureId, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureId, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public DoubleParser(string cultureName, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<double, bool> checkParsed)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty) 
        {
            mCheckParsed = checkParsed;
        }
        public DoubleParser(int cultureId, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<double, bool> checkParsed)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out double value, out string formatted)
        {
            value = 0;
            formatted = string.Empty;

            try
            {
                PrepareEntry(ref entry);

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!double.TryParse(entry, mStyles, mCulture, out value))
                    return ParseResult.ParseError;

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                value = 0;
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParse(string entry, out double value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            double val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            double val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            double val;
            return TryParse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToDouble(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion
    }
    public sealed class DecimalParser : NumericParserBase
    {
        #region Private Felder
        private Func<decimal, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public Func<decimal, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public DecimalParser(string cultureName, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureName, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public DecimalParser(int cultureId, bool showThousandsSep, byte decimals,
            byte leadingZeros, bool allowEmpty)
            : base(cultureId, showThousandsSep, decimals, leadingZeros, allowEmpty) { }
        public DecimalParser(string cultureName, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<decimal, bool> checkParsed)
            : base(cultureName, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        public DecimalParser(int cultureId, byte decimals, byte leadingZeros,
            bool allowEmpty, Func<decimal, bool> checkParsed)
            : base(cultureId, false, decimals, leadingZeros, allowEmpty)
        {
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out decimal value, out string formatted)
        {
            value = 0;
            formatted = string.Empty;

            try
            {
                PrepareEntry(ref entry);

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!decimal.TryParse(entry, mStyles, mCulture, out value))
                    return ParseResult.ParseError;

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                value = 0;
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParse(string entry, out decimal value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            decimal val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            decimal val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            decimal val;
            return TryParse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToDecimal(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion

        public string Formatted(DateTime value)
        {
            return string.Format(mCulture, "{0:" + mPattern + "}", value);
        }
    }
    #endregion

    #region DateParser und TimeParser
    public sealed class DateParser : ParserBase
    {
        #region Private Felder
        private DateFormat mDateFormat;
        private string[] mDefaultPatterns;
        private string[] mShortDatePatterns;
        private string[] mLongDatePatterns;
        private Regex mRegex;
        private Func<DateTime, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public DateFormat DateFormat
        {
            get { return mDateFormat; }
        }
        #region Öffentliche Eigenschaften
        public Func<DateTime, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion
        #endregion

        #region Öffentliche Konstruktoren
        public DateParser(string cultureName, DateFormat dateFormat, bool allowEmpty)
            : base(cultureName, allowEmpty) 
        {
            SetParams(dateFormat);
        }
        public DateParser(int cultureId, DateFormat dateFormat, bool allowEmpty)
            : base(cultureId, allowEmpty)
        {
            SetParams(dateFormat);
        }
        public DateParser(string cultureName, DateFormat dateFormat, bool allowEmpty,
            Func<DateTime, bool> checkParsed)
            : base(cultureName, allowEmpty)
        {
            mCheckParsed = checkParsed;
            SetParams(dateFormat);
        }
        public DateParser(int cultureId, DateFormat dateFormat, bool allowEmpty,
            Func<DateTime, bool> checkParsed)
            : base(cultureId, allowEmpty)
        {
            mCheckParsed = checkParsed;
            SetParams(dateFormat);
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult Parse(string entry, out DateTime value, out string formatted)
        {
            value = new DateTime();
            formatted = string.Empty;

            try
            {
                if (entry == null)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                entry = entry.Trim();

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (!DateTime.TryParseExact(mRegex.Replace(entry, " "), mDefaultPatterns, mCulture, DateTimeStyles.None, out value) &&
                    !DateTime.TryParseExact(entry, mShortDatePatterns, mCulture, DateTimeStyles.AllowInnerWhite, out value) &&
                    !DateTime.TryParseExact(entry, mLongDatePatterns, mCulture, DateTimeStyles.AllowInnerWhite, out value))
                {
                    return ParseResult.ParseError;
                }

                formatted = string.Format(mCulture, "{0:" + mPattern + "}", value);

                if (mCheckParsed != null && !mCheckParsed(value))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch 
            {   
                value = new DateTime();
                formatted = string.Empty;
                return ParseResult.ParseError;
            }
        }
        public ParseResult Parse(string entry, out DateTime value)
        {
            string formatted;
            return Parse(entry, out value, out formatted);
        }
        public string Formatted(DateTime value)
        {
            return string.Format(mCulture, "{0:" + mPattern + "}", value);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            DateTime val;
            ParseResult res = Parse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            DateTime val;
            string formatted;
            ParseResult res = Parse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            DateTime val;
            return Parse(entry, out val, out formatted);
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                entry = string.Format(mCulture, "{0:" + mPattern + "}",
                    Convert.ToDateTime(value, mCulture));

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion

        #region Private Methoden
        private void SetParams(DateFormat dateFormat)
        {
            string year;

            if (dateFormat == DateFormat.ShortDate_yy)
            {
                mDateFormat = dateFormat;
                year = "yy";
            }
            else
            {
                mDateFormat = DateFormat.ShortDate_yyyy;
                year = "yyyy";
            }

            string basePattern = mCulture.DateTimeFormat.ShortDatePattern;
            string dateOrder = string.Empty;

            for (int i = 0; i < basePattern.Length; i++)
            {
                if (basePattern[i] == 'd' && dateOrder.IndexOf('d') < 0)
                    dateOrder += 'd';
                else if (basePattern[i] == 'M' && dateOrder.IndexOf('m') < 0)
                    dateOrder += 'm';
                else if (basePattern[i] == 'y' && dateOrder.IndexOf('y') < 0)
                    dateOrder += 'y';

                if (dateOrder.Length == 3)
                    break;
            }

            if (dateOrder == "dmy")
            {
                mPattern = "dd/MM/" + year;

                mDefaultPatterns = new string[]
                {
                    "dd MM yyyy", "dd MM yy",
                    "d MM yyyy", "d MM yy",
                    "d M yyyy", "d M yy",
                    "dd MM", "dd M",
                    "d MM", "d M",
                    "ddMMyyyy", "ddMMyy",
                    "ddMM"
                };
            }
            else if (dateOrder == "mdy")
            {
                mPattern = "MM/dd/" + year;

                mDefaultPatterns = new string[]
                {
                    "MM dd yyyy", "MM dd yy",
                    "M dd yyyy", "M dd yy",
                    "M d yyyy", "M d yy",
                    "MM dd", "MM d",
                    "M dd", "M d",
                    "MMddyyyy", "MMddyy",
                    "MMdd"
                };
            }
            else if (dateOrder == "ymd")
            {
                mPattern = year + "/MM/dd";

                mDefaultPatterns = new string[]
                {
                    "yyyy MM dd", "yy MM dd",
                    "yyyy MM d", "yy MM d",
                    "yyyy M d", "yy M d",
                    "MM dd", "MM d",
                    "M dd", "M d",
                    "yyyyMMdd", "yyMMdd",
                    "MMdd"
                };
            }
            else if (dateOrder == "dym")
            {
                mPattern = "dd/" + year + "/MM";

                mDefaultPatterns = new string[]
                {
                    "dd yyyy MM", "dd yy MM",
                    "d yyyy MM", "d yy MM",
                    "d yyyy M", "d yy M",
                    "ddyyyyMM", "ddyyMM"
                };
            }
            else if (dateOrder == "myd")
            {
                mPattern = "MM/" + year + "/dd";

                mDefaultPatterns = new string[]
                {
                    "MM yyyy dd", "MM yy dd",
                    "MM yyyy d", "MM yy d",
                    "M yyyy d", "M yy d",
                    "MMyyyydd", "MMyydd"
                };
            }
            else if (dateOrder == "ydm")
            {
                mPattern = year + "/dd/MM";

                mDefaultPatterns = new string[]
                {
                    "yyyy dd MM", "yy dd MM",
                    "yyyy d MM", "yy d MM",
                    "yyyy d M", "yy d M",
                    "yyyyddMM", "yyddMM"
                };
            }
            else
            {
                mPattern = mCulture.DateTimeFormat.ShortDatePattern;
                mDefaultPatterns = new string[] { };
            }

            mShortDatePatterns = mCulture.DateTimeFormat.GetAllDateTimePatterns('d');
            mLongDatePatterns = mCulture.DateTimeFormat.GetAllDateTimePatterns('D');

            string sep = Regex.Escape(mCulture.DateTimeFormat.DateSeparator);
            string pattern = @"(?<=\d+)\s*(" + sep + @"|\s+)\s*(?=\d+)";
            mRegex = new Regex(pattern, RegexOptions.Compiled);
        }
        #endregion
    }
    public sealed class TimeParser : ParserBase
    {
        #region Öffentliche Enums
        public enum BaseType : int
        {
            DateTime,
            TimeSpan,
            LongTicks,
            IntSeconds,
            IntMinutes
        }
        #endregion

        #region Private Felder
        private static readonly string[] mDefaultPatterns = new string[] 
        {
            "HH mm", "H mm", "H m", "HH m",
            "HH mm ss", "HH mm s", "HH m ss", "HH m s",
            "H mm ss", "H mm s", "H m ss", "H m s",
            "HH", "HHmm", "HHmmss"
        };
        private string[] mNetPatterns;
        private DateTime mBaseDate = new DateTime().Date;
        private TimeFormat mTimeFormat;
        private BaseType mFormatType;
        private BaseType mParseType;
        private Regex mRegex;
        private Func<object, string> GetFormat;
        private Func<DateTime, object> GetObject;
        private Func<TimeSpan, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public DateTime BaseDate
        {
            get { return mBaseDate; }
        }
        public TimeFormat TimeFormat
        {
            get { return mTimeFormat; }
        }
        public BaseType FormatType
        {
            get { return mFormatType; }
        }
        public BaseType ParseType
        {
            get { return mParseType; }
        }
        #region Öffentliche Eigenschaften
        public Func<TimeSpan, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion
        #endregion

        #region Konstruktoren
        public TimeParser(string cultureName, TimeFormat timeFormat,
            BaseType formatType, BaseType parseType, bool allowEmpty)
            : base(cultureName, allowEmpty) 
        {
            SetParams(timeFormat, formatType, parseType);
        }
        public TimeParser(int cultureId, TimeFormat timeFormat,
            BaseType formatType, BaseType parseType, bool allowEmpty)
            : base(cultureId, allowEmpty)
        {
            SetParams(timeFormat, formatType, parseType);
        }
        public TimeParser(string cultureName, TimeFormat timeFormat,
            BaseType formatType, BaseType parseType, bool allowEmpty,
            Func<TimeSpan, bool> checkParsed)
            : base(cultureName, allowEmpty)
        {
            mCheckParsed = checkParsed;
            SetParams(timeFormat, formatType, parseType);
        }
        public TimeParser(int cultureId, TimeFormat timeFormat,
            BaseType formatType, BaseType parseType, bool allowEmpty,
            Func<TimeSpan, bool> checkParsed)
            : base(cultureId, allowEmpty)
        {
            mCheckParsed = checkParsed;
            SetParams(timeFormat, formatType, parseType);
        }
        #endregion

        #region Öffentliche Methoden
        //
        public void SetBaseDate(DateTime date)
        {
            mBaseDate = date.Date;
        }
        //
        public ParseResult TryParseAsDateTime(string entry, out DateTime value, out string formatted)
        {
            formatted = string.Empty;
            value = mBaseDate;

            try
            {
                if (entry == null)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                entry = entry.Trim();

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (entry.Length == 1 && char.IsDigit(entry[0]))
                    entry = "0" + entry;

                DateTime date;

                bool success =
                    DateTime.TryParseExact(mRegex.Replace(entry, " "),
                    mDefaultPatterns, mCulture, DateTimeStyles.None, out date);

                if (!success)
                {
                    success =
                        DateTime.TryParseExact(entry, mNetPatterns, mCulture,
                        DateTimeStyles.AllowWhiteSpaces, out date);
                }

                if (!success)
                    return ParseResult.ParseError;

                value = mBaseDate.AddTicks(date.TimeOfDay.Ticks);
                formatted = date.ToString(mPattern, mCulture);

                if (mCheckParsed != null && !mCheckParsed(value.TimeOfDay))
                    return ParseResult.CheckError;

                return ParseResult.Success;
            }
            catch
            {
                formatted = string.Empty;
                value = mBaseDate;
                return ParseResult.ParseError;
            }
        }
        public ParseResult TryParseAsTimeSpan(string entry, out TimeSpan value, out string formatted)
        {
            DateTime val;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = val.TimeOfDay;
            return res;
        }
        //
        public ParseResult TryParseAsDateTime(string entry, out DateTime value)
        {
            string formatted;

            return TryParseAsDateTime(entry, out value, out formatted);
        }
        public ParseResult TryParseAsTimeSpan(string entry, out TimeSpan value)
        {
            DateTime val;
            string formatted;
     
            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = val.TimeOfDay;
            return res;
        }
        //
        public ParseResult TryParseAsTicks(string entry, out long value, out string formatted)
        {
            DateTime val;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = val.TimeOfDay.Ticks;
            return res;
        }
        public ParseResult TryParseAsTicks(string entry, out long value)
        {
            DateTime val;
            string formatted;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = val.TimeOfDay.Ticks;
            return res;
        }
        //
        public ParseResult TryParseAsSeconds(string entry, out int value, out string formatted)
        {
            DateTime val;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = (int)(val.TimeOfDay.Ticks / TimeSpan.TicksPerSecond);
            return res;
        }
        public ParseResult TryParseAsSeconds(string entry, out int value)
        {
            DateTime val;
            string formatted;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = (int)(val.TimeOfDay.Ticks / TimeSpan.TicksPerSecond);
            return res;
        }
        //
        public ParseResult TryParseAsMinutes(string entry, out int value, out string formatted)
        {
            DateTime val;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = (int)(val.TimeOfDay.Ticks / TimeSpan.TicksPerMinute);
            return res;
        }
        public ParseResult TryParseAsMinutes(string entry, out int value)
        {
            DateTime val;
            string formatted;

            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = (int)(val.TimeOfDay.Ticks / TimeSpan.TicksPerMinute);
            return res;
        }
        //
        #endregion

        #region Öffentliche überschriebene Methoden
        //
        public override ParseResult TryParse(string entry, out string formatted)
        {
            DateTime value;
            return TryParseAsDateTime(entry, out value, out formatted);
        }
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            DateTime val;
            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = GetObject(val);
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            DateTime val;
            string formatted;
            ParseResult res = TryParseAsDateTime(entry, out val, out formatted);
            value = GetObject(val);
            return res;
        }
        //
        public override FormatResult TryFormat(object value, out string formatted)
        {
            formatted = string.Empty;

            try
            {
                if (value == null || value == DBNull.Value)
                    return FormatResult.Empty;

                formatted = GetFormat(value);
                return FormatResult.Success;
            }
            catch
            {
                formatted = string.Empty;
                return FormatResult.Error;
            }
        }
        //
        #endregion

        #region Private Methoden
        private void SetParams(TimeFormat timeFormat, BaseType formatType, BaseType parseType)
        {
            SetTimeFormat(timeFormat);
            SetFormatType(formatType);
            SetParseType(parseType);
            SetNetPatterns();
            SetRegex();
        }
        private void SetTimeFormat(TimeFormat timeFormat)
        {
            if (timeFormat == TimeFormat.HH_mm_ss)
            {
                mTimeFormat = timeFormat;
                mPattern = "HH:mm:ss";
            }
            else
            {
                mTimeFormat = TimeFormat.HH_mm;
                mPattern = "HH:mm";
            }
        }
        private void SetFormatType(BaseType formatType)
        {
            switch (formatType)
            {
                case BaseType.DateTime:
                    mFormatType = formatType;
                    GetFormat = (value) =>
                    {
                        DateTime date = Convert.ToDateTime(value);
                        return date.ToString(mPattern, mCulture);
                    };
                    break;
                case BaseType.LongTicks:
                    mFormatType = formatType;
                    GetFormat = (value) =>
                    {
                        long ticks = Convert.ToInt64(value) % TimeSpan.TicksPerDay;
                        return new DateTime(ticks).ToString(mPattern, mCulture);
                    };
                    break;
                case BaseType.IntSeconds:
                    mFormatType = formatType;
                    GetFormat = (value) =>
                    {
                        long ticks = (Convert.ToInt32(value) * TimeSpan.TicksPerSecond) % TimeSpan.TicksPerDay;
                        return new DateTime(ticks).ToString(mPattern, mCulture);
                    };
                    break;
                case BaseType.IntMinutes:
                    mFormatType = formatType;
                    GetFormat = (value) =>
                    {
                        long ticks = (Convert.ToInt32(value) * TimeSpan.TicksPerMinute) % TimeSpan.TicksPerDay;
                        return new DateTime(ticks).ToString(mPattern, mCulture);
                    };
                    break;
                default:
                    mFormatType = BaseType.TimeSpan;
                    GetFormat = (value) =>
                    {
                        long ticks = ((TimeSpan)value).Ticks % TimeSpan.TicksPerDay;
                        return new DateTime(ticks).ToString(mPattern, mCulture);
                    };
                    break;
            }
        }
        private void SetParseType(BaseType parseType)
        {
            switch (parseType)
            {
                case BaseType.DateTime:
                    mParseType = parseType;
                    GetObject = (value) => { return value; };
                    break;
                case BaseType.LongTicks:
                    mParseType = parseType;
                    GetObject = (value) => { return value.TimeOfDay.Ticks; };
                    break;
                case BaseType.IntSeconds:
                    mParseType = parseType;
                    GetObject = (value) => { return (int)(value.TimeOfDay.Ticks / TimeSpan.TicksPerSecond); };
                    break;
                case BaseType.IntMinutes:
                    mParseType = parseType;
                    GetObject = (value) => { return (int)(value.TimeOfDay.Ticks / TimeSpan.TicksPerMinute); };
                    break;
                default:
                    mParseType = BaseType.TimeSpan;
                    GetObject = (value) => { return value.TimeOfDay; };
                    break;
            }
        }
        private void SetNetPatterns()
        {
            List<string> temp = new List<string>(10);
            temp.AddRange(mCulture.DateTimeFormat.GetAllDateTimePatterns('t'));
            temp.AddRange(mCulture.DateTimeFormat.GetAllDateTimePatterns('T'));
            mNetPatterns = temp.ToArray();
        }
        private void SetRegex()
        {
            string sep = Regex.Escape(mCulture.DateTimeFormat.TimeSeparator);
            string pattern = @"(?<=\d+)\s*(" + sep + @"|\s+)\s*(?=\d+)";
            mRegex = new Regex(pattern, RegexOptions.Compiled);
        }
        #endregion
    }
    #endregion

    #region BooleanParser
    public sealed class BooleanParser : ParserBase
    {
        #region Felder
        private string mTrueString;
        private string mFalseString;
        #endregion

        #region Eigenschaften
        public string TrueString
        {
            get { return mTrueString; }
        }
        public string FalseString
        {
            get { return mFalseString; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public BooleanParser(string cultureName, string trueString,
            string falseString, bool allowEmpty)
            : base(cultureName, allowEmpty)
        {
            SetParams(trueString, falseString);
        }
        public BooleanParser(int cultureId, string trueString,
            string falseString, bool allowEmpty)
            : base(cultureId, allowEmpty)
        {
            SetParams(trueString, falseString);
        }
        #endregion

        #region Öffentliche Methoden
        public ParseResult TryParse(string entry, out bool value, out string formatted)
        {
            value = false;
            formatted = string.Empty;

            if (entry == null)
                return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

            entry = entry.Trim();

            if (entry == string.Empty)
                return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

            if (string.Equals(mTrueString, entry, StringComparison.OrdinalIgnoreCase))
            {
                value = true;
                formatted = mTrueString;
                return ParseResult.Success;
            }

            if (string.Equals(mFalseString, entry, StringComparison.OrdinalIgnoreCase))
            {
                formatted = mFalseString;
                return ParseResult.Success;
            }

            return ParseResult.ParseError;
        }
        public ParseResult TryParse(string entry, out bool value)
        {
            string formatted;
            return TryParse(entry, out value, out formatted);
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        //
        public override ParseResult TryParse(string entry, out object value, out string formatted)
        {
            bool val;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            bool val;
            string formatted;
            ParseResult res = TryParse(entry, out val, out formatted);
            value = val;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            bool val;
            return TryParse(entry, out val, out formatted);
        }
        //
        public override FormatResult TryFormat(object value, out string formatted)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    formatted = string.Empty;
                    return FormatResult.Empty;
                }

                if (Convert.ToBoolean(value, mCulture))
                    formatted = mTrueString;
                else
                    formatted = mFalseString;

                return FormatResult.Success;
            }
            catch
            {
                formatted = string.Empty;
                return FormatResult.Error;
            }
        }
        //
        #endregion

        #region Private Methoden
        private void SetParams(string trueString, string falseString)
        {
            if (trueString == null)
                mTrueString = string.Empty;
            else
                mTrueString = trueString.Trim();

            if (falseString == null)
                mFalseString = string.Empty;
            else
                mFalseString = falseString.Trim();
        }
        #endregion
    }
    #endregion

    #region StringParser
    public sealed class StringParser : ParserBase
    {
        #region Private Felder
        private bool mTrim;
        private Func<string, bool> mCheckParsed;
        #endregion

        #region Öffentliche Eigenschaften
        public bool Trim
        {
            get { return mTrim; }
        }
        public Func<string, bool> CheckParsed
        {
            get { return mCheckParsed; }
        }
        #endregion

        #region Öffentliche Konstruktoren
        public StringParser(string cultureName, bool trim, bool allowEmpty)
            : base(cultureName, allowEmpty)
        {
            mTrim = trim;
        }
        public StringParser(int cultureId, bool trim, bool allowEmpty)
            : base(cultureId, allowEmpty)
        {
            mTrim = trim;
        }
        public StringParser(string cultureName, bool trim, bool allowEmpty,
            Func<string, bool> checkParsed)
            : base(cultureName, allowEmpty)
        {
            mTrim = trim;
            mCheckParsed = checkParsed;
        }
        public StringParser(int cultureId, bool trim, bool allowEmpty,
            Func<string, bool> checkParsed)
            : base(cultureId, allowEmpty)
        {
            mTrim = trim;
            mCheckParsed = checkParsed;
        }
        #endregion

        #region Öffentliche überschriebene Methoden
        public override ParseResult TryParse(string entry, out object value,
            out string formatted)
        {
            ParseResult res = TryParse(entry, out formatted);
            value = formatted;
            return res;
        }
        public override ParseResult TryParse(string entry, out object value)
        {
            string formatted;
            ParseResult res = TryParse(entry, out formatted);
            value = formatted;
            return res;
        }
        public override ParseResult TryParse(string entry, out string formatted)
        {
            formatted = string.Empty;

            try
            {
                if (entry == null)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (mTrim)
                    formatted = entry.Trim();

                if (entry == string.Empty)
                    return mAllowEmpty ? ParseResult.Empty : ParseResult.EmptyError;

                if (mCheckParsed != null && !mCheckParsed(formatted))
                    return ParseResult.CheckError;

                return formatted == string.Empty ? ParseResult.Empty : ParseResult.Success;
            }
            catch { return ParseResult.ParseError; }
        }
        public override FormatResult TryFormat(object value, out string entry)
        {
            try
            {
                if (value == DBNull.Value || value == null)
                {
                    entry = string.Empty;
                    return FormatResult.Empty;
                }

                if(mTrim)
                    entry = Convert.ToString(value, mCulture).Trim();
                else
                    entry = Convert.ToString(value, mCulture);

                return FormatResult.Success;
            }
            catch
            {
                entry = string.Empty;
                return FormatResult.Error;
            }
        }
        #endregion
    }
    #endregion
}
