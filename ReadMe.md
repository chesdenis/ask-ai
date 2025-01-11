# AI Text Writer

## App features
- App with argument parsing;
- Markdown content only - easy to read and use;
- Writes answer directly in physical file using specific LLM api;
- Supports batch processing with progress indication;
- Can be configured as part of OS pipeline (MacOS Automator, Windows Batch, etc) for set of physical files
- Auto generation of file name based on input prompt
- Summary preparation based on prompts and answers
- File links and folders support to get proper examples and context

## IMPORTANT!!
- This app is not LLM, it is just proxy/wrapper which helps to use LLM inside any editor, like vs code or notepad or npp++ 
- This app expects that you have access to LLM API and you have valid API key
- This app is not responsible for:
  - any misuse of LLM API 
  - any data loss or data corruption
  - any security issues
  - any legal issues
  - any financial issues
  - any other issues
- You are responsible for your own actions and decisions

## How to organize AI workspace
First you need organize correctly your AI folder of prompt. Of course app can work without this, 
but good structuring will help you to easily navigate between your prompts.
Here is an example of good structure:
- I have folder AI in my Documents folder
- Inside AI folder I have folders related to different context, like:
  - cloud
  - bicycle
  - programming
  - cooking
  - etc
- In each folder i have specific empty files without extensions to reflect my contextual tags
  - For example in cloud folder I have files: aws, lambda, etc
  - In programming folder I have files: C#, dotnet, etc
  - In cooking folder I have files: meat, fish, etc
- These files will be collected by application and provided as context to each prompt for LLM

## How to use AskAI
### Answers formats:
  - History here [your-file-name].history.md - this is all history of your conversation with AI model;
  - Answer in [your-file-name].answer.md - this is last answer from AI model;
  - Short summary in [your-file-name].summary.md - this is summary of all your conversation;

### How to ask simple question:
  - Run AskAI and type your general question, press enter and see results in created md file inside temp folder. If you want you can move that file to your workspace
  - See answers and summary files

### How to ask question with context:
  - Create/Open any md file in your workspace (see "How to organize AI workspace") and put your question, for example:
  ```markdown
    Describe me something about cycling
  ```
  - See answers and summary files

### How to ask sequence of questions with context
  - Create/Open any md file in your workspace (see "How to organize AI workspace") and put your question, for example:
  ```markdown
    Describe me something about cycling
    ---
    I also wants to see more detail about carbon bicycle
    ---
    Expand me weight aspects
  ```
  - Save file and run AskAI for that file using this format: AskAI <path to file>
  - See answers and summary files

### How to reference to particular file or folder
  - Create/Open any md file in your workspace (see "How to organize AI workspace") and put your question, for example:
  ```markdown
  I have some bicycle text description here @path-to-bicycle-description-with-text and set of usage cases in this folder @path-to-usage-cases-text
  ---
  I also wants to see more detail potential upgrade of this bicycle
  ```
  - See answers and summary files