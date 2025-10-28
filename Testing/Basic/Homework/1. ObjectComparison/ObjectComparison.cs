using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public partial class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar_IgnoringIds()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        //можно оставить так, но вдруг появиться свойство, название которого будет начинаться с Id, или иметь Id в середине наименования.
        /*actualTsar
            .Should()
            .BeEquivalentTo(expectedTsar, opt => 
            opt.Excluding(info => info.Path.EndsWith("Id")));*/
        
        actualTsar
            .Should()
            .BeEquivalentTo(expectedTsar, opt =>
            opt.Excluding((IMemberInfo memberInfo) => MyRegex().IsMatch(memberInfo.Name)));
    }
    
    
     /*
     * 1. Если упадет тест мы увидим сообщение из разряда:
     * Expected: True
     * But was: False
     * что не является информативным сообщением, и мы не поймем в чем ошибка конкретно.
     * 2. Методе AreEqual реализует то, что уже имеют библиотеки, например .BeEquivalentTo, т.е. мы изобретаем велосипед заново.
     * 3. Возвращаем булевый результат, из за чего теряем контекст ошибки(аналогично п.1)
     * 4. Метод жестко проверяет свойство parent, и если добавить новое свойство, например Child, тогда тест будет падать, либо нужно будет добавлять новые проверки
     * 5. Если добавлять впринципе новые свойства/поля тест упадет, или нужно будет добавлять новые проверки
     * 6. Может уйти в бесконечную рекурсию, если ссылка parent будет идти обратно к исходному обьекту.
     */
    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }

    [GeneratedRegex(@"(^Id\b|\bId$)")]
    private static partial Regex MyRegex();
}
