namespace aemarcoCommons.ToolboxTesting;

public static class TestDataFile
{
    public static FileInfo GetFixture(params string[] relativePathParts) =>
        new(Path.Combine([TestContext.CurrentContext.TestDirectory, .. relativePathParts]));

    public static FileInfo GetEmptyFilesFixture(string category, string name) =>
        new(Path.Combine(TestContext.CurrentContext.TestDirectory, "EmptyFiles", category, name));
}