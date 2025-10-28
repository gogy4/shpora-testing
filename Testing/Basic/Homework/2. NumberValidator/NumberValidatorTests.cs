using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    #region Конструктор

    [TestCase(-1, 2)]
    [TestCase(1, -1)]
    [TestCase(3, 3)]
    [TestCase(3, 4)]
    public void Constructor_ShouldThrow_WhenArgsInvalid(int precision, int scale)
    {
        var act = () => new NumberValidator(precision, scale);
        act.Should().Throw<ArgumentException>($"precision={precision}, scale={scale} недопустимы");
    }

    [TestCase(1, 0, true)]
    [TestCase(10, 5, false)]
    public void Constructor_ShouldNotThrow_WhenArgsValid(int precision, int scale, bool onlyPositive)
    {
        var act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().NotThrow($"precision={precision}, scale={scale}, onlyPositive={onlyPositive} допустимы");
    }

    #endregion

    #region Валидация чисел

    public static IEnumerable ValidNumbers => new[]
    {
        new TestCaseData(17, 2, true, "0.0").SetName("Valid_0.0"),
        new TestCaseData(4, 2, true, "+1.23").SetName("Valid_+1.23"),
        new TestCaseData(4, 2, false, "-1.23").SetName("Valid_-1.23"),
        new TestCaseData(5, 0, true, "12345").SetName("Valid_Integer")
    };

    [Test, TestCaseSource(nameof(ValidNumbers))]
    public void IsValidNumber_ShouldReturnTrue_ForValidNumbers(int precision, int scale, bool onlyPositive,
        string input)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator
            .IsValidNumber(input)
            .Should()
            .BeTrue(
                $"Тест '{TestContext.CurrentContext.Test.Name}' провален: " +
                $"'{input}' должно быть валидным для precision={precision}, scale={scale}");
    }

    public static IEnumerable InvalidNumbers => new[]
    {
        new TestCaseData(3, 2, true, "+0.00", "число превышает допустимое количество знаков").SetName("Invalid_+0.00"),
        new TestCaseData(7, 2, true, "-1.231", "число превышает допустимое количество знаков после запятой").SetName(
            "Invalid_-1.231"),
        new TestCaseData(3, 2, true, "a.sd", "содержит недопустимые символы").SetName("Invalid_Chars"),
        new TestCaseData(3, 2, true, "", "пустая строка").SetName("Invalid_Empty"),
        new TestCaseData(3, 2, true, " ", "строка содержит только пробел").SetName("Invalid_Space"),
        new TestCaseData(3, 2, true, "+", "строка содержит только знак без числа").SetName("Invalid_SignOnly"),
        new TestCaseData(3, 2, true, "-", "строка содержит только знак без числа").SetName("Invalid_SignOnlyMinus"),
        new TestCaseData(2, 0, true, "211", "число превышает допустимую длину целой части").SetName(
            "Invalid_TooLongInteger"),
        new TestCaseData(3, 0, true, "0.1", "число превышает допустимую длину дробной части").SetName(
            "Invalid_ScaleExceeds"),
        new TestCaseData(17, 2, true, "0.000", "число превышает допустимую длину дробной части").SetName(
            "Invalid_ScaleExceeds2"),
        new TestCaseData(3, 0, true, "0.-12", "недопустимый знак внутри числа").SetName("Invalid_InternalSign"),
        new TestCaseData(3, 0, true, "0.+12", "недопустимый знак внутри числа").SetName("Invalid_InternalSignPlus"),
        new TestCaseData(3, 0, true, "0.12.12", "число содержит несколько разделителей").SetName(
            "Invalid_MultipleSeparators"),
        new TestCaseData(3, 0, true, "0.,12.12", "число содержит несколько разделителей").SetName(
            "Invalid_MixedSeparators"),
        new TestCaseData(3, 0, true, "0.2,12", "число содержит несколько разделителей").SetName(
            "Invalid_MixedSeparators2"),
        new TestCaseData(3, 0, true, "0,2.12", "число содержит несколько разделителей").SetName(
            "Invalid_MixedSeparators3"),
        new TestCaseData(3, 2, true, null, "значение null недопустимо").SetName("Invalid_Null")
    };

    [Test, TestCaseSource(nameof(InvalidNumbers))]
    public void IsValidNumber_ShouldReturnFalse_ForInvalidNumbers(int precision, int scale, bool onlyPositive,
        string input, string expectedMessage)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator
            .IsValidNumber(input)
            .Should()
            .BeFalse(
                $"Тест '{TestContext.CurrentContext.Test.Name}' провален: " +
                $"'{input ?? "null"}'. {expectedMessage}. " + $"Параметры: precision={precision}, scale={scale}");
    }

    #endregion
}