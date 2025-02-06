using NetCord.Gateway;
using NetCord.Rest;

namespace SharkbotNetCord.ImageGeneration
{
    public class GenerateImageResponseService
    {
        public GenerateImageResponseService()
        {
        }

        public async void GenerateImageResponse(Message message, string imagePath)
        {
            using var imageStream = new FileStream(imagePath, FileMode.Open);
            var attachment = new AttachmentProperties("image.jpg", imageStream);
            var reply = new ReplyMessageProperties();
            reply.AddAttachments(attachment);
            await message.ReplyAsync(reply);
        }
    }
}
