## Motivation

Azure Devops currently only supports export of single wiki pages to PDF. I wanted to migrate an entire WIKI.

## Prerequisite

You must have:

- Chrome installed with a version that maches the `chromedriver.exe` in this project. The current version is 99.0.4844.82

- Access to the WIKI page and have an ongoing session

When running the program:

- You must fill out `AZURE_DEVOPS_WIKI_URL` and `CHROME_USER_PROFILE` in `Program.cs`. 
The Wiki URL can be found by navigating to the DevOps project and opening the main wiki page.

- You must close all open chrome windows

## How does it work?

The project uses Selenium to automatically use Azure DevOps' print function. It builds a directory hierachy that maches the sidemenu and can handle
any level of nesting. To handle the native print dialog the `InputSimulatorCore` library is used to press enter and fill out absolute path.

## Shortcomings

I found it necessary to add a couple of explicit waits to ensure the pages have been rendered before the print dialog attempts to print an empty page. I haven't
spent looking into it but one solution would be to use Selenium's `Explicit Wait` rather than hardcoding a wait with `Thread.Sleep`


