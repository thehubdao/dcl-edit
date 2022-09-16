using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class ExposedNamesTest
    {
        [Test]
        public void CheckExposedNames()
        {
            Debug.Log("-------------------------------");
            CheckName("Hello world!");
            CheckName("gutentag");
            CheckName("12345");
            CheckName("switch");
            CheckName("HSUAOEUCUETZE");
            CheckName("§!%&/()=?\"}][{³²+#-.,*':;~ `°^´");
            CheckName("H S U A O E U C U E T Z E");
            CheckName("?öéö/óÖúü§Äò3èù46èß");
            CheckName("i&f");
        }

        private void CheckName(string name)
        {
            Debug.Log($"\"{name}\" -> \"{ExposeEntitySystem.ExposedNameFromName(name)}\"");
        }

        [Test]
        public void NoChanges()
        {
            // Just letters
            Assert.AreEqual("HelloWorld",ExposeEntitySystem.ExposedNameFromName("HelloWorld"));
            // Letters and numbers
            Assert.AreEqual("hi1234", ExposeEntitySystem.ExposedNameFromName("hi1234"));
            // Letters and underscore
            Assert.AreEqual("hi_there", ExposeEntitySystem.ExposedNameFromName("hi_there"));
            // Letters and dollar
            Assert.AreEqual("hi$there", ExposeEntitySystem.ExposedNameFromName("hi$there"));
            // underscore at start
            Assert.AreEqual("_hi", ExposeEntitySystem.ExposedNameFromName("_hi"));
            // dollar at start
            Assert.AreEqual("$hi", ExposeEntitySystem.ExposedNameFromName("$hi"));
        }

        [Test]
        public void ForbiddenChars()
        {
            // Space
            Assert.AreEqual("hithere", ExposeEntitySystem.ExposedNameFromName("hi there"));
            // Tab
            Assert.AreEqual("hithere", ExposeEntitySystem.ExposedNameFromName("hi\tthere"));
            // Newline
            Assert.AreEqual("hithere", ExposeEntitySystem.ExposedNameFromName("hi\nthere"));
            // special chars
            Assert.AreEqual("hithere", ExposeEntitySystem.ExposedNameFromName("hi!\"§%&/()=?`´^°*th+~#'-.:,;<>|\\}ere][{"));
        }

        [Test]
        public void StartsWithDigit()
        {
            // digit and letters
            Assert.AreEqual("_1234hi", ExposeEntitySystem.ExposedNameFromName("1234hi"));
            // only digit
            Assert.AreEqual("_1234", ExposeEntitySystem.ExposedNameFromName("1234"));
        }

        /*
        // Reserved Words
        break
        case
        catch
        class
        const
        continue
        debugger
        default
        delete
        do
        else
        enum
        export
        extends
        false
        finally
        for
        function
        if
        import
        in
        instanceof
        new
        null
        return
        super
        switch
        this
        throw
        true
        try
        typeof
        var
        void
        while
        with
        as
        implements
        interface
        let
        package
        private
        protected
        public
        static
        yield
        any
        boolean
        constructor
        declare
        get
        module
        require
        number
        set
        string
        symbol
        type
        from
        of
         */
        [Test]
        public void IsReserved()
        {
            Assert.AreEqual("_break", ExposeEntitySystem.ExposedNameFromName("break"));
            Assert.AreEqual("_case", ExposeEntitySystem.ExposedNameFromName("case"));
            Assert.AreEqual("_catch", ExposeEntitySystem.ExposedNameFromName("catch"));
            Assert.AreEqual("_class", ExposeEntitySystem.ExposedNameFromName("class"));
            Assert.AreEqual("_const", ExposeEntitySystem.ExposedNameFromName("const"));
            Assert.AreEqual("_continue", ExposeEntitySystem.ExposedNameFromName("continue"));
            Assert.AreEqual("_debugger", ExposeEntitySystem.ExposedNameFromName("debugger"));
            Assert.AreEqual("_default", ExposeEntitySystem.ExposedNameFromName("default"));
            Assert.AreEqual("_delete", ExposeEntitySystem.ExposedNameFromName("delete"));
            Assert.AreEqual("_do", ExposeEntitySystem.ExposedNameFromName("do"));
            Assert.AreEqual("_else", ExposeEntitySystem.ExposedNameFromName("else"));
            Assert.AreEqual("_enum", ExposeEntitySystem.ExposedNameFromName("enum"));
            Assert.AreEqual("_export", ExposeEntitySystem.ExposedNameFromName("export"));
            Assert.AreEqual("_extends", ExposeEntitySystem.ExposedNameFromName("extends"));
            Assert.AreEqual("_false", ExposeEntitySystem.ExposedNameFromName("false"));
            Assert.AreEqual("_finally", ExposeEntitySystem.ExposedNameFromName("finally"));
            Assert.AreEqual("_for", ExposeEntitySystem.ExposedNameFromName("for"));
            Assert.AreEqual("_function", ExposeEntitySystem.ExposedNameFromName("function"));
            Assert.AreEqual("_if", ExposeEntitySystem.ExposedNameFromName("if"));
            Assert.AreEqual("_import", ExposeEntitySystem.ExposedNameFromName("import"));
            Assert.AreEqual("_in", ExposeEntitySystem.ExposedNameFromName("in"));
            Assert.AreEqual("_instanceof", ExposeEntitySystem.ExposedNameFromName("instanceof"));
            Assert.AreEqual("_new", ExposeEntitySystem.ExposedNameFromName("new"));
            Assert.AreEqual("_null", ExposeEntitySystem.ExposedNameFromName("null"));
            Assert.AreEqual("_return", ExposeEntitySystem.ExposedNameFromName("return"));
            Assert.AreEqual("_super", ExposeEntitySystem.ExposedNameFromName("super"));
            Assert.AreEqual("_switch", ExposeEntitySystem.ExposedNameFromName("switch"));
            Assert.AreEqual("_this", ExposeEntitySystem.ExposedNameFromName("this"));
            Assert.AreEqual("_throw", ExposeEntitySystem.ExposedNameFromName("throw"));
            Assert.AreEqual("_true", ExposeEntitySystem.ExposedNameFromName("true"));
            Assert.AreEqual("_try", ExposeEntitySystem.ExposedNameFromName("try"));
            Assert.AreEqual("_typeof", ExposeEntitySystem.ExposedNameFromName("typeof"));
            Assert.AreEqual("_var", ExposeEntitySystem.ExposedNameFromName("var"));
            Assert.AreEqual("_void", ExposeEntitySystem.ExposedNameFromName("void"));
            Assert.AreEqual("_while", ExposeEntitySystem.ExposedNameFromName("while"));
            Assert.AreEqual("_with", ExposeEntitySystem.ExposedNameFromName("with"));
            Assert.AreEqual("_as", ExposeEntitySystem.ExposedNameFromName("as"));
            Assert.AreEqual("_implements", ExposeEntitySystem.ExposedNameFromName("implements"));
            Assert.AreEqual("_interface", ExposeEntitySystem.ExposedNameFromName("interface"));
            Assert.AreEqual("_let", ExposeEntitySystem.ExposedNameFromName("let"));
            Assert.AreEqual("_package", ExposeEntitySystem.ExposedNameFromName("package"));
            Assert.AreEqual("_private", ExposeEntitySystem.ExposedNameFromName("private"));
            Assert.AreEqual("_protected", ExposeEntitySystem.ExposedNameFromName("protected"));
            Assert.AreEqual("_public", ExposeEntitySystem.ExposedNameFromName("public"));
            Assert.AreEqual("_static", ExposeEntitySystem.ExposedNameFromName("static"));
            Assert.AreEqual("_yield", ExposeEntitySystem.ExposedNameFromName("yield"));
            Assert.AreEqual("_any", ExposeEntitySystem.ExposedNameFromName("any"));
            Assert.AreEqual("_boolean", ExposeEntitySystem.ExposedNameFromName("boolean"));
            Assert.AreEqual("_constructor", ExposeEntitySystem.ExposedNameFromName("constructor"));
            Assert.AreEqual("_declare", ExposeEntitySystem.ExposedNameFromName("declare"));
            Assert.AreEqual("_get", ExposeEntitySystem.ExposedNameFromName("get"));
            Assert.AreEqual("_module", ExposeEntitySystem.ExposedNameFromName("module"));
            Assert.AreEqual("_require", ExposeEntitySystem.ExposedNameFromName("require"));
            Assert.AreEqual("_number", ExposeEntitySystem.ExposedNameFromName("number"));
            Assert.AreEqual("_set", ExposeEntitySystem.ExposedNameFromName("set"));
            Assert.AreEqual("_string", ExposeEntitySystem.ExposedNameFromName("string"));
            Assert.AreEqual("_symbol", ExposeEntitySystem.ExposedNameFromName("symbol"));
            Assert.AreEqual("_type", ExposeEntitySystem.ExposedNameFromName("type"));
            Assert.AreEqual("_from", ExposeEntitySystem.ExposedNameFromName("from"));
            Assert.AreEqual("_of", ExposeEntitySystem.ExposedNameFromName("of"));
        }

        [Test]
        public void ResultingInReserved()
        {
            // with space
            Assert.AreEqual("_string", ExposeEntitySystem.ExposedNameFromName("str ing"));
            // with other special characters
            Assert.AreEqual("_string", ExposeEntitySystem.ExposedNameFromName("str!ing"));
        }
    }
}
