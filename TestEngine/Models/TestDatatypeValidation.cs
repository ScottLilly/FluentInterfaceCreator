using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using Engine.Resources;
using Shouldly;
using Xunit;

namespace TestEngine.Models
{
    public class TestDatatypeValidation
    {
        [Fact]
        public void Test_NameIsEmptyNamespaceIsEmpty()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("", "");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeTrue();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameContainsInternalSpaceNamespaceIsEmpty()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asd asd", "");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeTrue();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameContainsInternalSpaceAndSpecialCharacterNamespaceIsEmpty()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asd asd#", "");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeTrue();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeTrue();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameContainsSpecialCharacterNamespaceIsEmpty()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asdasd#", "");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeTrue();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameValidNamespaceIsEmpty()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asdasd", "");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameValidNamespaceContainsInternalSpace()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asdasd", "asd asd");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeTrue();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeFalse();
        }

        [Fact]
        public void Test_NameValidNamespaceContainsInternalSpaceAndSpecialCharacter()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asdasd", "asd asd#");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeTrue();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeTrue();
        }

        [Fact]
        public void Test_NameValidNamespaceContainsSpecialCharacter()
        {
            Datatype datatype = Datatype.BuildCustomDatatype("asdasd", "asdasd#");

            List<string> errors = datatype.ValidationErrors().ToList();

            errors.Contains(ErrorMessages.DatatypeIsRequired).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NameCannotContainSpecialCharacters).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceCannotContainAnInternalSpace).ShouldBeFalse();
            errors.Contains(ErrorMessages.NamespaceIsNotValid).ShouldBeTrue();
        }
    }
}