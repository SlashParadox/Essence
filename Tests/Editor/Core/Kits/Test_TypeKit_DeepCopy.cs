// Copyright (c) Craig Williams, SlashParadox

// Copyright (c) 2014 Burtsev Alexey
// Copyright (c) 2019 Jean-Paul Mikkers
// Based on https://github.com/jpmikkers/Baksteen.Extensions.DeepCopy, by Mikkers. License included.

#nullable enable
using NUnit.Framework;
using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

public class Test_TypeKit_DeepCopy
{
    private class MySingleObject : IEquatable<MySingleObject?>
    {
        public string One = "single one";

        public int Two { get; set; } = 2;

        public override bool Equals(object? obj)
        {
            return Equals(obj as MySingleObject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(One, Two, Two);
        }

        public bool Equals(MySingleObject? other)
        {
            return other is not null &&
                   One == other.One &&
                   Two == other.Two &&
                   Two == other.Two;
        }
    }

    private class MyNestedObject
    {
        public readonly MySingleObject Single = new MySingleObject();
        public readonly string Meta = "metadata";
    }

    private class OverriddenHash
    {
        public override int GetHashCode()
        {
            return 42;
        }
    }

    /// <summary>
    /// Encapsulates an object, the container will always be seen as a mutable ref type.
    /// Simplifies testing deepcopying.
    /// </summary>
    /// <typeparam name="T">Type to be encapsulated</typeparam>
    private class Wrapper<T> : IEquatable<Wrapper<T>>
    {
        public T Value { get; set; } = default!;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Wrapper<T>);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<T>.Default.GetHashCode(Value!);
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        public bool Equals(Wrapper<T>? other)
        {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }

    private static IEnumerable<T> ToIEnumerable<T>(System.Collections.IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return (T)enumerator.Current;
        }
    }

    private static void AssertArraysAreEqual<T>(Array array1, Array array2, bool refsMustBeDifferent)
    {
        Assert.AreEqual(array1.GetType(), array2.GetType());
        Assert.AreEqual(array1.LongLength, array2.LongLength);

        var counts1 = Enumerable.Range(0, array1.Rank).Select(array1.GetLongLength).ToArray();
        var counts2 = Enumerable.Range(0, array2.Rank).Select(array2.GetLongLength).ToArray();

        foreach (var (First, Second) in counts1.Zip(counts2, (l1, l2) => (l1, l2)))
        {
            Assert.AreEqual(First, Second);
        }

        foreach (var (x, y) in ToIEnumerable<T>(array1).Zip(ToIEnumerable<T>(array2), (arg1, arg2) => (arg1, arg2)))
        {
            Assert.AreEqual(x, y);
            if (refsMustBeDifferent) Assert.AreNotSame(x, y);
        }
    }

    [Test]
    public void PrimitiveTest()
    {
        Assert.IsTrue(typeof(bool).IsPrimitive);
        Assert.IsTrue(typeof(byte).IsPrimitive);
        Assert.IsTrue(typeof(sbyte).IsPrimitive);
        Assert.IsTrue(typeof(char).IsPrimitive);
        Assert.IsTrue(typeof(short).IsPrimitive);
        Assert.IsTrue(typeof(ushort).IsPrimitive);
        Assert.IsTrue(typeof(int).IsPrimitive);
        Assert.IsTrue(typeof(uint).IsPrimitive);
        Assert.IsTrue(typeof(nint).IsPrimitive);
        Assert.IsTrue(typeof(nuint).IsPrimitive);
        Assert.IsTrue(typeof(long).IsPrimitive);
        Assert.IsTrue(typeof(ulong).IsPrimitive);
        Assert.IsTrue(typeof(float).IsPrimitive);
        Assert.IsTrue(typeof(double).IsPrimitive);
    }

    [Test]
    public void NonPrimitiveTest()
    {
        Assert.IsFalse(typeof(object).IsPrimitive);
        Assert.IsFalse(typeof(string).IsPrimitive);
        Assert.IsFalse(typeof(decimal).IsPrimitive);
        Assert.IsFalse(typeof(Complex).IsPrimitive);
        Assert.IsFalse(typeof(BigInteger).IsPrimitive);
        Assert.IsFalse(typeof(Guid).IsPrimitive);
        Assert.IsFalse(typeof(DateTime).IsPrimitive);
        Assert.IsFalse(typeof(TimeSpan).IsPrimitive);
        Assert.IsFalse(typeof(DateTimeOffset).IsPrimitive);
        Assert.IsFalse(typeof(Range).IsPrimitive);
        Assert.IsFalse(typeof(Index).IsPrimitive);
    }

    [Test]
    public void Copy_Null()
    {
        object? t1 = null;
        var t2 = t1.DeepCopy();
        Assert.IsNull(t2);
    }

    [Test]
    public void Copy_ShouldNotDeepCopyTypeInstances()
    {
        var t1 = typeof(MySingleObject);
        var t2 = t1.DeepCopy()!;
        Assert.AreSame(t1, t2);

        t1 = typeof(MySingleObject).GetType();
        t2 = t1.DeepCopy()!;
        Assert.AreSame(t1, t2);
    }

    [Test]
    public void Copy_ShouldNotDeepCopyTypeFields()
    {
        var t1 = new { TypeField = typeof(MySingleObject) };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        Assert.AreSame(t1.TypeField, t2.TypeField);
    }

    [Test]
    public void Copy_ShouldNotDeepCopyImmutableTypes()
    {
        static void SubTest(object? v)
        {
            Assert.AreSame(v, v.DeepCopy());
        }

        SubTest(new nint());
        SubTest(new nuint());
        SubTest(new bool());
        SubTest(new byte());
        SubTest(new sbyte());
        SubTest(new char());
        SubTest(new short());
        SubTest(new ushort());
        SubTest(new int());
        SubTest(new uint());
        SubTest(new long());
        SubTest(new ulong());
        SubTest(new float());
        SubTest(new double());
        SubTest(new decimal());
        SubTest(new Complex());
        SubTest(new Quaternion());
        SubTest(new Vector2());
        SubTest(new Vector3());
        SubTest(new Vector4());
        SubTest(new Plane());
        SubTest(new Matrix3x2());
        SubTest(new Matrix4x4());
        SubTest(new BigInteger());
        SubTest(new Guid());
        SubTest(new DateTime());
        SubTest(new TimeSpan());
        SubTest(new DateTimeOffset());
        SubTest(new Range());
        SubTest(new Index());
        SubTest(new string(""));
        SubTest(DBNull.Value);
        SubTest(new Version());
        SubTest(new Uri(@"http://localhost:80"));
    }

    [Test]
    public void Copy_XElementWithChildren()
    {
        XElement el = XElement.Parse(@"
                <root>
                    <child attrib='wow'>hi</child>
                    <child attrib='yeah'>hello</child>
                </root>");
        XElement copied = el.DeepCopy()!;

        var children = copied.Elements("child").ToList();
        Assert.AreEqual(2, children.Count);
        Assert.AreEqual("wow", children[0].Attribute("attrib")!.Value);
        Assert.AreEqual("hi", children[0].Value);

        Assert.AreEqual("yeah", children[1].Attribute("attrib")!.Value);
        Assert.AreEqual("hello", children[1].Value);
    }

    [Test]
    public void Copy_CopiesNestedObject()
    {
        MyNestedObject copied = new MyNestedObject();

        Assert.AreEqual("metadata", copied.Meta);
        Assert.AreEqual("single one", copied.Single.One);
        Assert.AreEqual(2, copied.Single.Two);
    }

    [Test]
    public void Copy_CopiesEnumerables()
    {
        IList<MySingleObject> list = new List<MySingleObject> { new MySingleObject { One = "1" }, new MySingleObject { One = "2" } };
        IList<MySingleObject> copied = list.DeepCopy()!;

        Assert.AreEqual(2, copied.Count);
        Assert.AreEqual("1", copied[0].One);
        Assert.AreEqual("2", copied[1].One);
    }

    [Test]
    public void Copy_CopiesSingleObject()
    {
        MySingleObject copied = new MySingleObject().DeepCopy()!;

        Assert.AreEqual("single one", copied.One);
        Assert.AreEqual(2, copied.Two);
    }

    [Test]
    public void Copy_CopiesSingleBuiltInObjects()
    {
        Assert.AreEqual("hello there", "hello there".DeepCopy());
        Assert.AreEqual(123, 123.DeepCopy());
    }

    [Test]
    public void Copy_CopiesSelfReferencingArray()
    {
        object[] arr = new object[1];
        arr[0] = arr;
        var copy = arr.DeepCopy()!;
        Assert.IsTrue(ReferenceEquals(copy, copy[0]));
    }

    [Test]
    public void ReferenceEqualityComparerShouldNotUseOverriddenHash()
    {
        var t = new OverriddenHash();
        var equalityComparer = new ReferenceEqualityComparer();
        Assert.AreNotEqual(42, equalityComparer.GetHashCode(t));
        Assert.AreEqual(equalityComparer.GetHashCode(t), RuntimeHelpers.GetHashCode(t));
    }

    [Test]
    public void Copy_Copies1dArray()
    {
        var t1 = new[] { 1, 2, 3 };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<int>(t1, t2, false);
    }

    [Test]
    public void Copy_Copies1dRefElementArray()
    {
        var t1 = new[] { new Wrapper<int> { Value = 1 }, new Wrapper<int> { Value = 2 }, new Wrapper<int> { Value = 3 } };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<Wrapper<int>>(t1, t2, true);
    }

    [Test]
    public void Copy_Copies2dArray()
    {
        var t1 = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<int>(t1, t2, false);
    }

    [Test]
    public void Copy_Copies2dRefElementArray()
    {
        var t1 = new[,] { { new Wrapper<int> { Value = 1 }, new Wrapper<int> { Value = 2 } }, { new Wrapper<int> { Value = 3 }, new Wrapper<int> { Value = 4 } }, { new Wrapper<int> { Value = 5 }, new Wrapper<int> { Value = 6 } } };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<Wrapper<int>>(t1, t2, true);
    }

    [Test]
    public void Copy_Copies3dArray()
    {
        var t1 = new[,,] { { { 1, 2 }, { 3, 4 }, { 5, 6 } }, { { 7, 8 }, { 9, 10 }, { 11, 12 } } };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<int>(t1, t2, false);
    }

    [Test]
    public void Copy_Copies3dRefElementArray()
    {
        var t1 = new[,,] { { { new Wrapper<int> { Value = 1 }, new Wrapper<int> { Value = 2 } }, { new Wrapper<int> { Value = 3 }, new Wrapper<int> { Value = 4 } }, { new Wrapper<int> { Value = 5 }, new Wrapper<int> { Value = 6 } } }, { { new Wrapper<int> { Value = 7 }, new Wrapper<int> { Value = 8 } }, { new Wrapper<int> { Value = 9 }, new Wrapper<int> { Value = 10 } }, { new Wrapper<int> { Value = 11 }, new Wrapper<int> { Value = 12 } } } };
        var t2 = t1.DeepCopy()!;
        Assert.AreNotSame(t1, t2);
        AssertArraysAreEqual<Wrapper<int>>(t1, t2, true);
    }

    [Test]
    public void Copy_DeepCopiesMutableFieldsOfValueTypes()
    {
        // ValueTuple itself is an immutable valuetype, MySingleObject is a mutable reference type
        var a = new ValueTuple<MySingleObject>(new MySingleObject());
        var b = a.DeepCopy()!;
        Assert.AreNotSame(a, b);
        Assert.AreNotSame(a.Item1, b.Item1);
        Assert.AreEqual(a.Item1, b.Item1);
        Assert.AreEqual(a, b);
    }

    [Test]
    public void Copy_ShallowCopiesImmutableFieldsOfValueTypes()
    {
        // ValueTuple itself is an immutable valuetype, string is an immutable reference type
        var a = new ValueTuple<string>("U0FGZSBpcyBTaGl0dHkgQWdpbGUgRm9yIEVudGVycHJpc2VzIQ==");
        var b = a.DeepCopy()!;
        Assert.AreNotSame(a, b);
        Assert.AreSame(a.Item1, b.Item1);
        Assert.AreEqual(a.Item1, b.Item1);
        Assert.AreEqual(a, b);
    }

    [Test]
    public void EnsureMutableValueTypesHaveNoReferenceFields([Values(typeof(Vector2), typeof(Quaternion))] Type testType)
    {
        var typeToReflect = testType;

        Assert.IsTrue(typeToReflect.IsValueType);

        while (typeToReflect.BaseType != null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                Assert.IsTrue(fieldInfo.FieldType.IsValueType || fieldInfo.FieldType.IsPrimitive);
            }

            typeToReflect = typeToReflect.BaseType;
        }
    }
}
