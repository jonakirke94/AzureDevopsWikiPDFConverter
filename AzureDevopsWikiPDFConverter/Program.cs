using AzureDevopsWikiPDFConverter;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WindowsInput;
using WindowsInput.Native;

string OUTPUT_DIR = "C:\\downloads\\";
string AZURE_DEVOPS_WIKI_URL = "https://dev.azure.com/{TENANT}/{PROJECT}/_wiki/wikis/{PROJECT_NAME}/{WIKI_ID}/{WIKI_NAME}";
string CHROME_USER_PROFILE = "\\Users\\Jonathan\\AppData\\Local\\Google\\Chrome\\User Data";

InputSimulator sim = new();

try
    {
    ChromeOptions chromeOptions = new();
    chromeOptions.AddArgument("--verbose");
    chromeOptions.AddArgument("--whitelisted-ips=''");
    chromeOptions.AddArgument($"--user-data-dir=C:{CHROME_USER_PROFILE}");

    var driver = new ChromeDriver(chromeOptions);

    EnsureDirExists(OUTPUT_DIR);

    driver.Navigate().GoToUrl(AZURE_DEVOPS_WIKI_URL);

    SaveFrontpage(driver, OUTPUT_DIR);

    var topLevelMenuList = driver.FindElements(SELECTORS.TOP_LEVEL_ITEMS);
    foreach (var toplevelMenu in topLevelMenuList)
    {
        SaveItemsRecursively(driver, toplevelMenu, path: OUTPUT_DIR, level: 2);
    }

    driver.Quit();

}
catch (Exception e) {
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}

void SaveItemsRecursively(ChromeDriver driver, IWebElement element, string path, int level)
{
    // Navigate to element
    element.Click();

    var nextLevel = level + 1;
    var subLevetItems = GetSubLevelItems(driver, level: nextLevel);
    var hasChildren = subLevetItems.Any();

    var currentTitle = element.FindElement(SELECTORS.ITEM_TITLE).Text;
    var absoluteFilePath = Path.Combine(path, currentTitle);

    if (hasChildren)
    {
        EnsureDirExists(absoluteFilePath);
    }

    OpenPrintDialog(driver);

    // Let page render before we attempt to print
    Thread.Sleep(1000);

    // If the current page has children, then the current item itself must be placed in the same directory as its children
    // Example:
    // Parent              ->   Parent/Parent.pdf
    //   - Children 1      ->   Parent/Children 1.pdf  
    //   - Children 2      ->   Parent/Children 2.pdf
    var absolutePathForCurrentItem = hasChildren ? Path.Combine(absoluteFilePath, currentTitle) : absoluteFilePath;
    HandlePrintDialog(absolutePath: absolutePathForCurrentItem);

    foreach (var sublevelMenu in subLevetItems)
    {
        SaveItemsRecursively(driver, sublevelMenu, path: absoluteFilePath, level: nextLevel);
    }
    
    if (hasChildren)
    {
        // Close the current menu
        element.FindElement(SELECTORS.ITEM_EXPANDABLE_ARROW).Click();
    }
}

IEnumerable<IWebElement> GetSubLevelItems(ChromeDriver driver, int level)
{
    try
    {
        return driver.FindElements(SELECTORS.SUB_LEVEL_ITEMS(level));
    }
    catch (Exception)
    {
        return new List<IWebElement>();
    
    }
}

void OpenPrintDialog(ChromeDriver driver)
{
    Thread.Sleep(1500);

    // Open menu
    driver.FindElement(SELECTORS.PRINT_MENU).Click();

    // Click print
    driver.FindElement(SELECTORS.PRINT_BUTTON).Click();
}

void HandlePrintDialog(string absolutePath)
{
    // Press save in print dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

    Thread.Sleep(1500);
    // Reset absolute path in directory dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.DELETE);

    // Enter absolute path in directory dialog
    sim.Keyboard.TextEntry(absolutePath);

    // Press save in directory dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
}

void EnsureDirExists(string dir) {      
    if (!Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }
}

void SaveFrontpage(ChromeDriver driver, string path)
{
    OpenPrintDialog(driver);

    // Let page render before we attempt to print
    Thread.Sleep(1000);

    var wikiFrontPagePath = Path.Combine(path, "Wiki");
    HandlePrintDialog(wikiFrontPagePath);
}