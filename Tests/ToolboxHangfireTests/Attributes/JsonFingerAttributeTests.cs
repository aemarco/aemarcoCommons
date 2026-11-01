using aemarcoCommons.ToolboxHangfire;
#pragma warning disable IDE0130

namespace ToolboxHangfireTests;

public class JsonFingerAttributeTests
{

    [Test]
    public void CreateSignature_NoProperties_SerializesWholeObject()
    {
        //arrange
        var attribute = new JsonFingerAttribute();
        var payload = new Payload(1, "a");

        //act
        var result = attribute.CreateSignature(payload);

        //assert
        result.ShouldBe("""{"Id":1,"Name":"a"}""");
    }

    [Test]
    public void CreateSignature_WithProperties_ListsNamesAndValues()
    {
        //arrange
        var attribute = new JsonFingerAttribute("Id", "Name");
        var payload = new Payload(1, "a");

        //act
        var result = attribute.CreateSignature(payload);

        //assert
        result.ShouldBe("Id_1_Name_a");
    }

    [Test]
    public void CreateSignature_UnknownProperty_KeepsNameWithoutValue()
    {
        //arrange
        var attribute = new JsonFingerAttribute("Id", "Missing");
        var payload = new Payload(1, "a");

        //act
        var result = attribute.CreateSignature(payload);

        //assert
        result.ShouldBe("Id_1_Missing");
    }

}
