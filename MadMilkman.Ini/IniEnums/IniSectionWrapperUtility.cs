namespace MadMilkman.Ini
{
    /* CONSIDER: Implement ToChar extension method on IniSectionWrapper instead of SectionWrapperToChar method.
     *           In order to define an extension method in .NET 2.0 we need to declare ExtensionAttribute our self.
     *           
     *           namespace System.Runtime.CompilerServices { public sealed class ExtensionAttribute : Attribute { } } */

    internal static class IniSectionWrapperUtility
    {
        public static char SectionWrapperToChar(IniSectionWrapper wrapper, bool toStartCharacter)
        {
            switch (wrapper)
            {
                case IniSectionWrapper.AngleBrackets:
                    return (toStartCharacter) ? '<' : '>';
                case IniSectionWrapper.CurlyBrackets:
                    return (toStartCharacter) ? '{' : '}';
                case IniSectionWrapper.Parentheses:
                    return (toStartCharacter) ? '(' : ')';
                default:
                    return (toStartCharacter) ? '[' : ']';
            }
        }
    }
}