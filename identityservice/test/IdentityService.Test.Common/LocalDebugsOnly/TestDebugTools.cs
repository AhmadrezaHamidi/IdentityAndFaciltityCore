namespace IdentityService.Test.Common.LocalDebugsOnly;

public static class RepeatUntilFail
{
    public static async Task Run(Func<Task> testBody)
    {
#if REPEAT_UNTIL_FAIL
        var iteration = 0;

        while (true)
        {
            iteration++;

            try
            {
                await testBody();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed on iteration: {iteration}");
                Console.WriteLine(ex);

                // if (Debugger.IsAttached)
                //     Debugger.Break();

                // throw;
            }
        }
#else
        await testBody();
#endif
    }

    public static void Run(Action testBody)
    {
#if REPEAT_UNTIL_FAIL
        var iteration = 0;

        while (true)
        {
            iteration++;

            try
            {
                testBody();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed on iteration: {iteration}");
                Console.WriteLine(ex);

                // if (Debugger.IsAttached)
                //     Debugger.Break();

                // throw;
            }
        }
#else
        testBody();
#endif
    }
}
