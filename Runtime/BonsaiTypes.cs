// Minimal Bonsai types for Unity compatibility
// These are stubs to satisfy Bonsai.Harp and Harp.Behavior dependencies

using System;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Bonsai
{
    /// <summary>
    /// Represents the category of a workflow element.
    /// </summary>
    public enum ElementCategory
    {
        Source,
        Transform,
        Sink,
        Combinator,
        Condition,
        Property,
        Nested,
        Workflow,
        Grammar,
        Scripting
    }

    /// <summary>
    /// Specifies the category of a workflow element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorkflowElementCategoryAttribute : Attribute
    {
        public WorkflowElementCategoryAttribute(ElementCategory category) => Category = category;
        public ElementCategory Category { get; }
    }

    /// <summary>
    /// Marks a class as containing combinator methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinatorAttribute : Attribute
    {
        public string MethodName { get; set; }
    }

    /// <summary>
    /// Provides a name identifier for a workflow element.
    /// </summary>
    public interface INamedElement
    {
        string Name { get; }
    }

    /// <summary>
    /// Base class for reactive sources.
    /// </summary>
    public abstract class Source<TResult>
    {
        public abstract IObservable<TResult> Generate();
    }

    /// <summary>
    /// Base class for transformations with one input.
    /// </summary>
    public abstract class Transform<TSource, TResult>
    {
        public abstract IObservable<TResult> Process(IObservable<TSource> source);
    }

    /// <summary>
    /// Base class for sinks.
    /// </summary>
    public abstract class Sink<TSource>
    {
        public abstract IObservable<TSource> Process(IObservable<TSource> source);
    }

    /// <summary>
    /// Base class for combinators.
    /// </summary>
    public abstract class Combinator<TSource, TResult>
    {
        public abstract IObservable<TResult> Process(IObservable<TSource> source);
    }

    /// <summary>
    /// Base class for combinators with same input/output type.
    /// </summary>
    public abstract class Combinator<TSource> : Combinator<TSource, TSource>
    {
    }

    /// <summary>
    /// Provides static helper methods for expression building.
    /// </summary>
    public static class DesignTypes
    {
        public const string UITypeEditor = "System.Drawing.Design.UITypeEditor, System.Drawing";
        public const string NumericUpDownEditor = "Bonsai.Design.NumericUpDownEditor, Bonsai.Design";
    }

    public abstract class CreateMessageBuilder
    {
        public object Payload { get; set; }
        public virtual string GetElementDisplayName(object element) => element.GetType().Name;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public RangeAttribute(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double Min { get; }
        public double Max { get; }
    }
}

namespace Bonsai.Harp
{
    public enum DeviceState : byte
    {
        Standby = 0,
        Active = 1,
        Speed = 3
    }

    public enum EnableType : byte
    {
        Disable = 0,
        Enable = 1
    }

    public enum ResetMode : byte
    {
        RestoreDefault = 0,
        RestoreEeprom = 1,
        Save = 2,
        RestoreName = 3
    }
}



namespace Bonsai.IO
{
    /// <summary>
    /// Helper class for path operations.
    /// </summary>
    public static class PathHelper
    {
        public static void EnsureDirectory(string path)
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }
    }
}
