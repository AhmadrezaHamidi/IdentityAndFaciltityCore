using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.JwtHelper;
using static OmidBank.Common.Enums;
using SkiaSharp;  // ← اضافه کن

namespace IdentityService.Application.Handler.Admin;

public class GenerateCaptchaQueryHandler : IBaseHandler<GenerateCaptchaQuery, BaseMessageResponse<GenerateCaptchaQueryResponse>>
{
    private readonly IJwtService _jwtService;
    private readonly int _expireMinutes;
    private readonly ILogger<GenerateCaptchaQueryHandler> _logger;
    private readonly IRedisCacheManager _redisCacheManager;
    public GenerateCaptchaQueryHandler(IJwtService jwtService, IConfiguration configuration,
        ILogger<GenerateCaptchaQueryHandler> logger, IRedisCacheManager redisCacheManager)
    {
        _jwtService = jwtService;
        _expireMinutes = configuration.GetValue<int>("Captcha:ExpireSeconds", defaultValue: 120);
        _logger = logger;
        _redisCacheManager = redisCacheManager;

    }

    public async Task<BaseMessageResponse<GenerateCaptchaQueryResponse>> Handle(GenerateCaptchaQuery request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GenerateCaptchaQueryResponse>(EResponseStatusCode.DoneSuccessfully);
        string captcha = GenerateRandomText();
        Guid token = Guid.NewGuid();
        _redisCacheManager.Write(string.Format(CacheKey.AdminCaptcha, token), captcha.ToLower(), ECacheTime.OneMinutes);
        response.Data = new GenerateCaptchaQueryResponse(token, GenerateCaptchaImage(captcha));
        return response;

    }

    private string GenerateRandomText(int length = 5)
    {
        var rnd = new Random();
        int min = (int)Math.Pow(10, length - 1);  // 10000
        int max = (int)Math.Pow(10, length);      // 100000
        return rnd.Next(min, max).ToString();
    }
    private byte[] GenerateCaptchaImage(string text, int width = 200, int height = 80)
    {
        var rnd = new Random();
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);


        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        using var font = new SKFont
        {
            Size = 40,
            Typeface = SKTypeface.Default
        };
        canvas.DrawText(text, 20, 55, font, paint);

        for (int i = 0; i < 50; i++)
        {
            var x = rnd.Next(width);
            var y = rnd.Next(height);
            canvas.DrawCircle(x, y, 1, paint);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

}
public record GenerateCaptchaQuery() : BaseCommand<BaseMessageResponse<GenerateCaptchaQueryResponse>>;

public sealed record GenerateCaptchaQueryResponse(Guid Token, byte[] Base64Image);