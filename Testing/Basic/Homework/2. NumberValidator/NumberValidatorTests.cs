using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    #region Конструктор

    [Test, TestCaseSource(nameof(InvalidArgs_NegativeOrScaleTooBig))]
    public void Constructor_ShouldThrow_WhenArgsIsNegativeOrScaleAtLeastPrecision(int precision, int scale, string expectedMessage)
    {
        var act = () => new NumberValidator(precision, scale);
        act
            .Should()
            .Throw<ArgumentException>($"{expectedMessage}. " + $"Параметры: precision={precision}, scale={scale}");
    }

    [TestCase(1, 0, true)]
    [TestCase(10, 5, false)]
    public void Constructor_DoesNotThrow_WhenPrecisionScaleAndOnlyPositiveAreValid(int precision, int scale, bool onlyPositive)
    {
        var act = () => new NumberValidator(precision, scale, onlyPositive);
        act
            .Should()
            .NotThrow($"precision={precision}, scale={scale}, onlyPositive={onlyPositive} допустимы");
    }

    #endregion

    #region Валидация чисел

    [Test, TestCaseSource(nameof(ValidNumbers))]
    public void IsValidNumber_ShouldReturnTrue_ForValidNumbers(int precision, int scale, bool onlyPositive,
        string input, string expectedMessage)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator
            .IsValidNumber(input)
            .Should()
            .BeTrue($"{input}. {expectedMessage}. " + $"Параметры: precision={precision}, scale={scale}, " +
                    $"onlyPositive={onlyPositive}");
    }

    [Test, TestCaseSource(nameof(InvalidNumbers))]
    public void IsValidNumber_ShouldReturnFalse_ForInvalidNumbers(int precision, int scale, bool onlyPositive,
        string input, string expectedMessage)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator
            .IsValidNumber(input)
            .Should()
            .BeFalse($"{input}. {expectedMessage}. " + $"Параметры: precision={precision}, scale={scale}, " +
                     $"onlyPositive={onlyPositive}");
    }

    #endregion

    public static IEnumerable<TestCaseData> InvalidNumbers()
    {
        yield return new TestCaseData(3, 2, true, "+0.00", "число превышает допустимое количество знаков")
            .SetName("InvalidNumber_TooManyDigits_PositiveZero");

        yield return new TestCaseData(7, 2, true, "-1.231",
                "число превышает допустимое количество знаков после запятой")
            .SetName("InvalidNumber_TooManyDecimals_Negative");

        yield return new TestCaseData(3, 2, true, "a.sd", "содержит недопустимые символы")
            .SetName("InvalidNumber_InvalidCharacters");

        yield return new TestCaseData(3, 2, true, "", "пустая строка")
            .SetName("InvalidNumber_EmptyString");

        yield return new TestCaseData(3, 2, true, " ", "строка содержит только пробел")
            .SetName("InvalidNumber_WhitespaceOnly");

        yield return new TestCaseData(3, 2, true, "+", "строка содержит только знак без числа")
            .SetName("InvalidNumber_SignOnlyPlus");

        yield return new TestCaseData(3, 2, true, "-", "строка содержит только знак без числа")
            .SetName("InvalidNumber_SignOnlyMinus");

        yield return new TestCaseData(2, 0, true, "211", "число превышает допустимую длину целой части")
            .SetName("InvalidNumber_IntegerTooLong");

        yield return new TestCaseData(3, 0, true, "0.1", "число превышает допустимую длину дробной части")
            .SetName("InvalidNumber_ScaleTooLong");

        yield return new TestCaseData(17, 2, true, "0.000", "число превышает допустимую длину дробной части")
            .SetName("InvalidNumber_ScaleTooLong2");

        yield return new TestCaseData(3, 0, true, "0.-12", "недопустимый знак внутри числа")
            .SetName("InvalidNumber_InternalSignMinus");

        yield return new TestCaseData(3, 0, true, "0.+12", "недопустимый знак внутри числа")
            .SetName("InvalidNumber_InternalSignPlus");

        yield return new TestCaseData(3, 0, true, "0.12.12", "число содержит несколько разделителей")
            .SetName("InvalidNumber_MultipleSeparators");

        yield return new TestCaseData(3, 0, true, "0.,12.12", "число содержит несколько разделителей")
            .SetName("InvalidNumber_MixedSeparators1");

        yield return new TestCaseData(3, 0, true, "0.2,12", "число содержит несколько разделителей")
            .SetName("InvalidNumber_MixedSeparators2");

        yield return new TestCaseData(3, 0, true, "0,2.12", "число содержит несколько разделителей")
            .SetName("InvalidNumber_MixedSeparators3");

        yield return new TestCaseData(3, 2, true, null, "значение null недопустимо")
            .SetName("InvalidNumber_NullValue");
    }


    public static IEnumerable<TestCaseData> ValidNumbers()
    {
        yield return new TestCaseData(
            17, 2, true, "0.0",
            "целая часть + дробная часть ≤ precision, дробная часть ≤ scale, число положительное"
        ).SetName("ValidNumber_ZeroPointZero");

        yield return new TestCaseData(
            4, 2, true, "+1.23",
            "целая часть + дробная часть ≤ precision, дробная часть ≤ scale, число положительное"
        ).SetName("ValidNumber_PositiveWithTwoDecimals");

        yield return new TestCaseData(
            4, 2, false, "-1.23",
            "целая часть + дробная часть ≤ precision, дробная часть ≤ scale, отрицательные числа разрешены"
        ).SetName("ValidNumber_NegativeWithTwoDecimals");

        yield return new TestCaseData(
            5, 0, true, "12345",
            "целая часть + дробная часть ≤ precision, дробная часть ≤ scale, число положительное"
        ).SetName("ValidNumber_Integer");
    }
    
    public static IEnumerable<TestCaseData> InvalidArgs_NegativeOrScaleTooBig()
    {
        yield return new TestCaseData(-1, 2, "отрицательный precision недопустим")
            .SetName("Constructor_Invalid_NegativePrecision");

        yield return new TestCaseData(1, -1, "отрицательный scale недопустим")
            .SetName("Constructor_Invalid_NegativeScale");

        yield return new TestCaseData(3, 3, "scale должен быть меньше чем precision")
            .SetName("Constructor_Invalid_ScaleEqualsPrecision");

        yield return new TestCaseData(3, 4, "scale должен быть меньше чем precision")
            .SetName("Constructor_Invalid_ScaleGreaterThanPrecision");
    }
}