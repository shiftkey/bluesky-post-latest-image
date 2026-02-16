using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using idunno.Bluesky;
using idunno.Bluesky.Embed;

var imagesDirectory = Environment.GetEnvironmentVariable("UPLOADS_DIRECTORY");
if (imagesDirectory == null)
{
    Console.WriteLine("Missing environment variable UPLOADS_DIRECTORY");
    Environment.Exit(1);
}

var allFiles = Directory.GetFiles(imagesDirectory, "*.jpg");

var latestImage = allFiles[^1];

var fileName = Path.GetFileName(latestImage);
var timestamp = fileName.Replace("image-", "").Replace(".jpg", "");

var dateTime = DateTime.ParseExact(timestamp, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
var currentTimeText = dateTime.ToString("H:mm tt on MMM d");

Console.WriteLine($"Date of latest image: {currentTimeText}");

// TODO: make this configurable?
var x = 230;
var y = 97;
var cropWidth = 1800;
var cropHeight = 1800;

byte[] imageAsBytes;

using (var outStream = new MemoryStream())
using (var image = Image.Load(latestImage))
{
    image.Mutate(i => i.Crop(new Rectangle(x, y, cropWidth, cropHeight)));

    image.SaveAsJpeg("output.jpg");
    image.SaveAsJpeg(outStream);
    imageAsBytes = outStream.ToArray();
}

BlueskyAgent agent = new();

var username = Environment.GetEnvironmentVariable("BLUESKY_USERNAME");
var password = Environment.GetEnvironmentVariable("BLUESKY_PASSWORD");

if (username == null)
{
    Console.WriteLine("Missing environment variable BLUESKY_USERNAME");
    Environment.Exit(1);
}

if (password == null)
{
    Console.WriteLine("Missing environment variable BLUESKY_PASSWORD");
    Environment.Exit(1);
}

CancellationToken cancellationToken = CancellationToken.None;

var loginResult = await agent.Login(username, password);
if (loginResult.Succeeded)
{
    var imageUploadResult = await agent.UploadImage(
        imageAsBytes,
        "image/jpg",
        "Observing the seedlings under the grow light",
        new AspectRatio(1000, 1000),
        cancellationToken: cancellationToken);

    if (imageUploadResult.Succeeded)
    {
        var response = await agent.Post(
          $"It is {currentTimeText} and not a lot is happening...",
          imageUploadResult.Result,
          cancellationToken: cancellationToken);

        if (response.Succeeded)
        {
            Console.WriteLine("Posted successfully");
        }
    }
}