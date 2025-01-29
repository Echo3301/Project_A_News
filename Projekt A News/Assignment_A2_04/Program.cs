using Assignment_A2_04.Models;
using Assignment_A2_04.Services;

namespace Assignment_A2_04;

class Program
{
    static async Task Main(string[] args)
    {
        //Create an instans of newsService
        var newsService = new NewsService();

        //Subscribe to the event
        newsService.NewsAvailable += OnNewsAvailable;

        //Array to hold tasks async
        Task<NewsResponse>[] tasks = { null, null, null, null, null };
        //Variable to catch any exception that might occur during execution
        Exception exception = null;

        try
        {
            //Geting news for each category
            tasks[0] = newsService.GetNewsAsync(NewsCategory.Entertainment);
            tasks[1] = newsService.GetNewsAsync(NewsCategory.Business);
            tasks[3] = newsService.GetNewsAsync(NewsCategory.Technology);
            tasks[2] = newsService.GetNewsAsync(NewsCategory.Sports);
            tasks[4] = newsService.GetNewsAsync(NewsCategory.World);

            await Task.WhenAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]);

            //Geting news for each category from XML cache
            tasks[0] = newsService.GetNewsAsync(NewsCategory.Entertainment);
            tasks[1] = newsService.GetNewsAsync(NewsCategory.Business);
            tasks[3] = newsService.GetNewsAsync(NewsCategory.Technology);
            tasks[2] = newsService.GetNewsAsync(NewsCategory.Sports);
            tasks[4] = newsService.GetNewsAsync(NewsCategory.World);

            await Task.WhenAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occurred: {ex.Message}");
        }
        try
        {
            Console.WriteLine(new string('-', 70));
            //Loop through each task
            foreach (var task in tasks)
            {
                //Check if the task has completed successfully
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    //Print headlines and latest news
                    Console.WriteLine($"\nLatest news in {task.Result.Category}:");
                    foreach (var art in task.Result.Articles)
                    {
                        Console.WriteLine($"   \t- {art.Title}");
                    }
                }
                //If the task encounters an error or faulted
                else if (task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine($"Task failed: {task.Exception.Message}");
                }
                else
                {
                    Console.WriteLine("News not avalible");
                }
            }
        }
        //This catch will catch any other exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occurred: {ex.Message}");
        }
    }
    static void OnNewsAvailable(object sender, string message)
    {
        Console.WriteLine(message);
    }
}

