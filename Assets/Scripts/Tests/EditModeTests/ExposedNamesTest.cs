using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class ExposedNamesTest
    {
        private ExposeEntitySystem _exposeEntitySystem;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _exposeEntitySystem = new ExposeEntitySystem();
        }

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
            Debug.Log($"\"{name}\" -> \"{_exposeEntitySystem.ExposedNameFromName(name)}\"");
        }

        [Test]
        public void NoChanges()
        {
            // Just letters
            Assert.AreEqual("HelloWorld", _exposeEntitySystem.ExposedNameFromName("HelloWorld"));
            // Letters and numbers
            Assert.AreEqual("hi1234", _exposeEntitySystem.ExposedNameFromName("hi1234"));
            // Letters and underscore
            Assert.AreEqual("hi_there", _exposeEntitySystem.ExposedNameFromName("hi_there"));
            // Letters and dollar
            Assert.AreEqual("hi$there", _exposeEntitySystem.ExposedNameFromName("hi$there"));
            // underscore at start
            Assert.AreEqual("_hi", _exposeEntitySystem.ExposedNameFromName("_hi"));
            // dollar at start
            Assert.AreEqual("$hi", _exposeEntitySystem.ExposedNameFromName("$hi"));
        }

        [Test]
        public void ForbiddenChars()
        {
            // Space
            Assert.AreEqual("hithere", _exposeEntitySystem.ExposedNameFromName("hi there"));
            // Tab
            Assert.AreEqual("hithere", _exposeEntitySystem.ExposedNameFromName("hi\tthere"));
            // Newline
            Assert.AreEqual("hithere", _exposeEntitySystem.ExposedNameFromName("hi\nthere"));
            // special chars
            Assert.AreEqual("hithere", _exposeEntitySystem.ExposedNameFromName("hi!\"§%&/()=?`´^°*th+~#'-.:,;<>|\\}ere][{"));
        }

        [Test]
        public void StartsWithDigit()
        {
            // digit and letters
            Assert.AreEqual("_1234hi", _exposeEntitySystem.ExposedNameFromName("1234hi"));
            // only digit
            Assert.AreEqual("_1234", _exposeEntitySystem.ExposedNameFromName("1234"));
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
            Assert.AreEqual("_break", _exposeEntitySystem.ExposedNameFromName("break"));
            Assert.AreEqual("_case", _exposeEntitySystem.ExposedNameFromName("case"));
            Assert.AreEqual("_catch", _exposeEntitySystem.ExposedNameFromName("catch"));
            Assert.AreEqual("_class", _exposeEntitySystem.ExposedNameFromName("class"));
            Assert.AreEqual("_const", _exposeEntitySystem.ExposedNameFromName("const"));
            Assert.AreEqual("_continue", _exposeEntitySystem.ExposedNameFromName("continue"));
            Assert.AreEqual("_debugger", _exposeEntitySystem.ExposedNameFromName("debugger"));
            Assert.AreEqual("_default", _exposeEntitySystem.ExposedNameFromName("default"));
            Assert.AreEqual("_delete", _exposeEntitySystem.ExposedNameFromName("delete"));
            Assert.AreEqual("_do", _exposeEntitySystem.ExposedNameFromName("do"));
            Assert.AreEqual("_else", _exposeEntitySystem.ExposedNameFromName("else"));
            Assert.AreEqual("_enum", _exposeEntitySystem.ExposedNameFromName("enum"));
            Assert.AreEqual("_export", _exposeEntitySystem.ExposedNameFromName("export"));
            Assert.AreEqual("_extends", _exposeEntitySystem.ExposedNameFromName("extends"));
            Assert.AreEqual("_false", _exposeEntitySystem.ExposedNameFromName("false"));
            Assert.AreEqual("_finally", _exposeEntitySystem.ExposedNameFromName("finally"));
            Assert.AreEqual("_for", _exposeEntitySystem.ExposedNameFromName("for"));
            Assert.AreEqual("_function", _exposeEntitySystem.ExposedNameFromName("function"));
            Assert.AreEqual("_if", _exposeEntitySystem.ExposedNameFromName("if"));
            Assert.AreEqual("_import", _exposeEntitySystem.ExposedNameFromName("import"));
            Assert.AreEqual("_in", _exposeEntitySystem.ExposedNameFromName("in"));
            Assert.AreEqual("_instanceof", _exposeEntitySystem.ExposedNameFromName("instanceof"));
            Assert.AreEqual("_new", _exposeEntitySystem.ExposedNameFromName("new"));
            Assert.AreEqual("_null", _exposeEntitySystem.ExposedNameFromName("null"));
            Assert.AreEqual("_return", _exposeEntitySystem.ExposedNameFromName("return"));
            Assert.AreEqual("_super", _exposeEntitySystem.ExposedNameFromName("super"));
            Assert.AreEqual("_switch", _exposeEntitySystem.ExposedNameFromName("switch"));
            Assert.AreEqual("_this", _exposeEntitySystem.ExposedNameFromName("this"));
            Assert.AreEqual("_throw", _exposeEntitySystem.ExposedNameFromName("throw"));
            Assert.AreEqual("_true", _exposeEntitySystem.ExposedNameFromName("true"));
            Assert.AreEqual("_try", _exposeEntitySystem.ExposedNameFromName("try"));
            Assert.AreEqual("_typeof", _exposeEntitySystem.ExposedNameFromName("typeof"));
            Assert.AreEqual("_var", _exposeEntitySystem.ExposedNameFromName("var"));
            Assert.AreEqual("_void", _exposeEntitySystem.ExposedNameFromName("void"));
            Assert.AreEqual("_while", _exposeEntitySystem.ExposedNameFromName("while"));
            Assert.AreEqual("_with", _exposeEntitySystem.ExposedNameFromName("with"));
            Assert.AreEqual("_as", _exposeEntitySystem.ExposedNameFromName("as"));
            Assert.AreEqual("_implements", _exposeEntitySystem.ExposedNameFromName("implements"));
            Assert.AreEqual("_interface", _exposeEntitySystem.ExposedNameFromName("interface"));
            Assert.AreEqual("_let", _exposeEntitySystem.ExposedNameFromName("let"));
            Assert.AreEqual("_package", _exposeEntitySystem.ExposedNameFromName("package"));
            Assert.AreEqual("_private", _exposeEntitySystem.ExposedNameFromName("private"));
            Assert.AreEqual("_protected", _exposeEntitySystem.ExposedNameFromName("protected"));
            Assert.AreEqual("_public", _exposeEntitySystem.ExposedNameFromName("public"));
            Assert.AreEqual("_static", _exposeEntitySystem.ExposedNameFromName("static"));
            Assert.AreEqual("_yield", _exposeEntitySystem.ExposedNameFromName("yield"));
            Assert.AreEqual("_any", _exposeEntitySystem.ExposedNameFromName("any"));
            Assert.AreEqual("_boolean", _exposeEntitySystem.ExposedNameFromName("boolean"));
            Assert.AreEqual("_constructor", _exposeEntitySystem.ExposedNameFromName("constructor"));
            Assert.AreEqual("_declare", _exposeEntitySystem.ExposedNameFromName("declare"));
            Assert.AreEqual("_get", _exposeEntitySystem.ExposedNameFromName("get"));
            Assert.AreEqual("_module", _exposeEntitySystem.ExposedNameFromName("module"));
            Assert.AreEqual("_require", _exposeEntitySystem.ExposedNameFromName("require"));
            Assert.AreEqual("_number", _exposeEntitySystem.ExposedNameFromName("number"));
            Assert.AreEqual("_set", _exposeEntitySystem.ExposedNameFromName("set"));
            Assert.AreEqual("_string", _exposeEntitySystem.ExposedNameFromName("string"));
            Assert.AreEqual("_symbol", _exposeEntitySystem.ExposedNameFromName("symbol"));
            Assert.AreEqual("_type", _exposeEntitySystem.ExposedNameFromName("type"));
            Assert.AreEqual("_from", _exposeEntitySystem.ExposedNameFromName("from"));
            Assert.AreEqual("_of", _exposeEntitySystem.ExposedNameFromName("of"));
        }

        [Test]
        public void ResultingInReserved()
        {
            // with space
            Assert.AreEqual("_string", _exposeEntitySystem.ExposedNameFromName("str ing"));
            // with other special characters
            Assert.AreEqual("_string", _exposeEntitySystem.ExposedNameFromName("str!ing"));
        }
    }
}
