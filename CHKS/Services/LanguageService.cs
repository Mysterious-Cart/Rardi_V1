namespace CHKS.Services;
public class LanguageService
{

    private readonly mydbService mydbService;

    public LanguageService(mydbService MydbService)
    {
        mydbService = MydbService;
    }

    public async Task GetLanguageData()
    {

    }
}