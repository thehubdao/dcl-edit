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
            Debug.Log($"\"{name}\" -> \"{_exposeEntitySystem.GenerateValidSymbol(name)}\"");
        }

        [Test]
        public void NoChanges()
        {
            // Just letters
            Assert.AreEqual("HelloWorld", _exposeEntitySystem.GenerateValidSymbol("HelloWorld"));
            // Letters and numbers
            Assert.AreEqual("hi1234", _exposeEntitySystem.GenerateValidSymbol("hi1234"));
            // Letters and underscore
            Assert.AreEqual("hi_there", _exposeEntitySystem.GenerateValidSymbol("hi_there"));
            // Letters and dollar
            Assert.AreEqual("hi$there", _exposeEntitySystem.GenerateValidSymbol("hi$there"));
            // underscore at start
            Assert.AreEqual("_hi", _exposeEntitySystem.GenerateValidSymbol("_hi"));
            // dollar at start
            Assert.AreEqual("$hi", _exposeEntitySystem.GenerateValidSymbol("$hi"));
        }

        [Test]
        public void ForbiddenChars()
        {
            // Space
            Assert.AreEqual("hithere", _exposeEntitySystem.GenerateValidSymbol("hi there"));
            // Tab
            Assert.AreEqual("hithere", _exposeEntitySystem.GenerateValidSymbol("hi\tthere"));
            // Newline
            Assert.AreEqual("hithere", _exposeEntitySystem.GenerateValidSymbol("hi\nthere"));
            // special chars
            Assert.AreEqual("hithere", _exposeEntitySystem.GenerateValidSymbol("hi!\"§%&/()=?`´^°*th+~#'-.:,;<>|\\}ere][{"));
        }

        [Test]
        public void StartsWithDigit()
        {
            // digit and letters
            Assert.AreEqual("_1234hi", _exposeEntitySystem.GenerateValidSymbol("1234hi"));
            // only digit
            Assert.AreEqual("_1234", _exposeEntitySystem.GenerateValidSymbol("1234"));
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
            Assert.AreEqual("_break", _exposeEntitySystem.GenerateValidSymbol("break"));
            Assert.AreEqual("_case", _exposeEntitySystem.GenerateValidSymbol("case"));
            Assert.AreEqual("_catch", _exposeEntitySystem.GenerateValidSymbol("catch"));
            Assert.AreEqual("_class", _exposeEntitySystem.GenerateValidSymbol("class"));
            Assert.AreEqual("_const", _exposeEntitySystem.GenerateValidSymbol("const"));
            Assert.AreEqual("_continue", _exposeEntitySystem.GenerateValidSymbol("continue"));
            Assert.AreEqual("_debugger", _exposeEntitySystem.GenerateValidSymbol("debugger"));
            Assert.AreEqual("_default", _exposeEntitySystem.GenerateValidSymbol("default"));
            Assert.AreEqual("_delete", _exposeEntitySystem.GenerateValidSymbol("delete"));
            Assert.AreEqual("_do", _exposeEntitySystem.GenerateValidSymbol("do"));
            Assert.AreEqual("_else", _exposeEntitySystem.GenerateValidSymbol("else"));
            Assert.AreEqual("_enum", _exposeEntitySystem.GenerateValidSymbol("enum"));
            Assert.AreEqual("_export", _exposeEntitySystem.GenerateValidSymbol("export"));
            Assert.AreEqual("_extends", _exposeEntitySystem.GenerateValidSymbol("extends"));
            Assert.AreEqual("_false", _exposeEntitySystem.GenerateValidSymbol("false"));
            Assert.AreEqual("_finally", _exposeEntitySystem.GenerateValidSymbol("finally"));
            Assert.AreEqual("_for", _exposeEntitySystem.GenerateValidSymbol("for"));
            Assert.AreEqual("_function", _exposeEntitySystem.GenerateValidSymbol("function"));
            Assert.AreEqual("_if", _exposeEntitySystem.GenerateValidSymbol("if"));
            Assert.AreEqual("_import", _exposeEntitySystem.GenerateValidSymbol("import"));
            Assert.AreEqual("_in", _exposeEntitySystem.GenerateValidSymbol("in"));
            Assert.AreEqual("_instanceof", _exposeEntitySystem.GenerateValidSymbol("instanceof"));
            Assert.AreEqual("_new", _exposeEntitySystem.GenerateValidSymbol("new"));
            Assert.AreEqual("_null", _exposeEntitySystem.GenerateValidSymbol("null"));
            Assert.AreEqual("_return", _exposeEntitySystem.GenerateValidSymbol("return"));
            Assert.AreEqual("_super", _exposeEntitySystem.GenerateValidSymbol("super"));
            Assert.AreEqual("_switch", _exposeEntitySystem.GenerateValidSymbol("switch"));
            Assert.AreEqual("_this", _exposeEntitySystem.GenerateValidSymbol("this"));
            Assert.AreEqual("_throw", _exposeEntitySystem.GenerateValidSymbol("throw"));
            Assert.AreEqual("_true", _exposeEntitySystem.GenerateValidSymbol("true"));
            Assert.AreEqual("_try", _exposeEntitySystem.GenerateValidSymbol("try"));
            Assert.AreEqual("_typeof", _exposeEntitySystem.GenerateValidSymbol("typeof"));
            Assert.AreEqual("_var", _exposeEntitySystem.GenerateValidSymbol("var"));
            Assert.AreEqual("_void", _exposeEntitySystem.GenerateValidSymbol("void"));
            Assert.AreEqual("_while", _exposeEntitySystem.GenerateValidSymbol("while"));
            Assert.AreEqual("_with", _exposeEntitySystem.GenerateValidSymbol("with"));
            Assert.AreEqual("_as", _exposeEntitySystem.GenerateValidSymbol("as"));
            Assert.AreEqual("_implements", _exposeEntitySystem.GenerateValidSymbol("implements"));
            Assert.AreEqual("_interface", _exposeEntitySystem.GenerateValidSymbol("interface"));
            Assert.AreEqual("_let", _exposeEntitySystem.GenerateValidSymbol("let"));
            Assert.AreEqual("_package", _exposeEntitySystem.GenerateValidSymbol("package"));
            Assert.AreEqual("_private", _exposeEntitySystem.GenerateValidSymbol("private"));
            Assert.AreEqual("_protected", _exposeEntitySystem.GenerateValidSymbol("protected"));
            Assert.AreEqual("_public", _exposeEntitySystem.GenerateValidSymbol("public"));
            Assert.AreEqual("_static", _exposeEntitySystem.GenerateValidSymbol("static"));
            Assert.AreEqual("_yield", _exposeEntitySystem.GenerateValidSymbol("yield"));
            Assert.AreEqual("_any", _exposeEntitySystem.GenerateValidSymbol("any"));
            Assert.AreEqual("_boolean", _exposeEntitySystem.GenerateValidSymbol("boolean"));
            Assert.AreEqual("_constructor", _exposeEntitySystem.GenerateValidSymbol("constructor"));
            Assert.AreEqual("_declare", _exposeEntitySystem.GenerateValidSymbol("declare"));
            Assert.AreEqual("_get", _exposeEntitySystem.GenerateValidSymbol("get"));
            Assert.AreEqual("_module", _exposeEntitySystem.GenerateValidSymbol("module"));
            Assert.AreEqual("_require", _exposeEntitySystem.GenerateValidSymbol("require"));
            Assert.AreEqual("_number", _exposeEntitySystem.GenerateValidSymbol("number"));
            Assert.AreEqual("_set", _exposeEntitySystem.GenerateValidSymbol("set"));
            Assert.AreEqual("_string", _exposeEntitySystem.GenerateValidSymbol("string"));
            Assert.AreEqual("_symbol", _exposeEntitySystem.GenerateValidSymbol("symbol"));
            Assert.AreEqual("_type", _exposeEntitySystem.GenerateValidSymbol("type"));
            Assert.AreEqual("_from", _exposeEntitySystem.GenerateValidSymbol("from"));
            Assert.AreEqual("_of", _exposeEntitySystem.GenerateValidSymbol("of"));
        }

        [Test]
        public void ResultingInReserved()
        {
            // with space
            Assert.AreEqual("_string", _exposeEntitySystem.GenerateValidSymbol("str ing"));
            // with other special characters
            Assert.AreEqual("_string", _exposeEntitySystem.GenerateValidSymbol("str!ing"));
        }
    }
}
