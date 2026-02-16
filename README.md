# `bluesky-post-latest-image`

A quick and dirty tool to post the latest image from a directory to a configured Bluesky account. This also does some cropping of the raw images to remove unnecessary elements.

Shout out to @blowdart for https://github.com/blowdart/idunno.Bluesky and @JimBobSquarePants for https://github.com/SixLabors/ImageSharp that act as the glue code for this workflow.

Example usage:

```
$ UPLOADS_DIRECTORY=/path/to/directory \
  HUMAN_DESCRIPTION="nothing much is happening" \
  BLUESKY_USERNAME=account.bsky.social \
  BLUESKY_PASSWORD=some-app-password \
  dotnet run
```
