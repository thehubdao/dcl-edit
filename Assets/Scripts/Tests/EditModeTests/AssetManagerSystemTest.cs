using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using NUnit.Framework;
using System;
using System.Linq;

public class AssetManagerSystemTest
{
    [Test]
    public void TestGetMetadataById()
    {
        AssetManagerSystem assetManagerSystem = new AssetManagerSystem();
        assetManagerSystem.Construct(
            new MockAssetLoader(
                new MockAssetLoader.TestData {filename = "model1.glb", type = FileAssetMetadata.AssetType.Model, id = Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")},
                new MockAssetLoader.TestData {filename = "image1.png", type = FileAssetMetadata.AssetType.Image, id = Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4")}
            )
        );

        var metadata = assetManagerSystem.GetMetadataById(Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
        Assert.NotNull(metadata);
        //Assert.AreEqual("model1.glb", metadata.assetFilename);
        Assert.AreEqual(AssetMetadata.AssetType.Model, metadata.assetType);
        Assert.AreEqual(Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), metadata.assetId);

        metadata = assetManagerSystem.GetMetadataById(Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
        Assert.NotNull(metadata);
        //Assert.AreEqual("image1.png", metadata.assetFilename);
        Assert.AreEqual(AssetMetadata.AssetType.Image, metadata.assetType);
        Assert.AreEqual(Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4"), metadata.assetId);
    }

    [Test]
    public void TestGetDataById()
    {
        AssetManagerSystem assetManagerSystem = new AssetManagerSystem();
        assetManagerSystem.Construct(
            new MockAssetLoader(
                new MockAssetLoader.TestData {filename = "model1.glb", type = FileAssetMetadata.AssetType.Model, id = Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")},
                new MockAssetLoader.TestData {filename = "image1.png", type = FileAssetMetadata.AssetType.Image, id = Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4")}
            )
        );

        var data = assetManagerSystem.GetDataById(Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
        Assert.NotNull(data);
        Assert.AreEqual(Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), data.id);
        Assert.AreEqual(AssetData.State.IsAvailable, data.state);
        Assert.IsInstanceOf(typeof(ModelAssetData), data);
        Assert.AreEqual("model1.glb", ((ModelAssetData)data).data.name);

        data = assetManagerSystem.GetDataById(Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
        Assert.NotNull(data);
        Assert.AreEqual(Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4"), data.id);
        Assert.AreEqual(AssetData.State.IsAvailable, data.state);
        Assert.IsInstanceOf(typeof(ImageAssetData), data);
    }

    [Test]
    public void TestGetAllAssetIds()
    {
        AssetManagerSystem assetManagerSystem = new AssetManagerSystem();
        assetManagerSystem.Construct(
            new MockAssetLoader(
                new MockAssetLoader.TestData {filename = "model1.glb", type = FileAssetMetadata.AssetType.Model, id = Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")},
                new MockAssetLoader.TestData {filename = "image1.png", type = FileAssetMetadata.AssetType.Image, id = Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4")}
            )
        );

        var ids = assetManagerSystem.GetAllAssetIds().ToList();
        Assert.Contains(Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), ids);
        Assert.Contains(Guid.Parse("AAAA8C5E-CEB2-4faa-B6BF-329BF39FA1E4"), ids);
    }
}
