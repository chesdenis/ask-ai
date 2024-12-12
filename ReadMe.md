# AI Text Writer

## App features
- Simple console app with argument parsing;
- Expected to work with Markdown content only;
- Writes answer directly in physical file using specific LLM api;
- Supports batch processing with progress indication;
- Can be configured as part of OS pipeline (MacOS Automator, Windows Batch, etc) for set of physical files
- Auto generation of file name based on input prompt
- Summary preparation based on prompts and answers

## IMPORTANT!!
- This app is not LLM, it is just proxy/wrapper which helps to use LLM inside any editor, like vs code or notepad or npp++ 
- This app expects that you have access to LLM API and you have valid API key
- This app is not responsible for any misuse of LLM API
- This app is not responsible for any data loss or data corruption
- This app is not responsible for any security issues
- This app is not responsible for any legal issues
- This app is not responsible for any financial issues
- This app is not responsible for any other issues
- You are responsible for your own actions and decisions

## How to use
First you need organize correctly your AI folder of prompt. Of course app can work without this, 
but good structuring will help you to easily navigate between your prompts.
Here is an example of good structure:
- I have folder AI in my Documents folder
- Inside AI folder I have folders related to different purpose, like:
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

### Listen:
  - Run `AITextWriter.Listen` with arguments:
    - `--working-folder` - path to working folder
    - `--model` - model name (optional, if empty will be used default depending on LLM api)
    - `--verbose` - show output (optional, default is `false`)
    - `--help` - show help
  - Create any file with any file name in the working folder
    - Type your prompt in Markdown format. 
    - Save your file.
    - In a few seconds and you will see the answer from LLM inside [your-file-name.your-extension].answer.md
    - You can open that file and see the answer and generated LLM content.
    - If you need additional prompt - put >3 new lines and type your new prompt or you can edit existing prompt.
    - Also check logs in console app if you have any issues
  - App will listen for changes and write answer and [your-file-name.your-extension].answer.md into same folder where you file with questions locates
  - Continue editing and saving file to get new/updated answers.
  - Context tags will be collected as physical files without extension in working folder
    - For example:
      - working folder is `C:\my-aws-related-prompts`
      - inside working folder we have these empty files without extensions: aws, C#, dotnet, lambda
      - create new file 'my-first-question.md' (name can be any) with content: 'Write me hello world'
      - file content my-first-question.md.answer.md will be transformed to these rows:
        - '### user'
        - 'Please use this context: aws, C#, dotnet, lambda'
        - 'Write me hello world'
        - '### assistant'
        - 'some answer from LLM which help you to write hello world as lambda function in AWS' 
      - it is good practice to have multiple working folder for different context and technologies to see relevant answers

### Process:
  - Prompt creation rules is same as for ListenChanges, but you need to call AITextWriter.ProcessSingle to get results:
    - Run `AITextWriter.Process` with arguments:
      - `--file` - path to working file
      - `--model` - model name (optional, if empty will be used default depending on LLM api, but if file name contains model name it will be used)
      - `--verbose` - show output (optional, default is `false`)
      - `--help` - show help
  - Single processing mode is useful when you need to handle batch of prompts in one go:
    - For example:
      - you have > 10 files inside context folder with tags, which contains some prompts
      - you can iterate through all files and call AITextWriter.ProcessSingle for each file to get answers
      - you can collect and analyze responses in other technologies or tools like DataSpell, Excel, PowerBI... depending on prompt results

### Summarize:
  - Run `AITextWriter.Summarize` with arguments:
    - `--working-folder` - path to working folder
    - `--file` - path to working file
    - `--model` - model name (optional, if empty will be used default depending on LLM api)
    - `--verbose` - show output (optional, default is `false`)
    - `--help` - show help
  - Summarize mode is useful when you need to collect all prompts and answers in one file:
    - For example:
      - you have > 10 files inside context folder with tags, which contains some prompts and answers
      - you can call AITextWriter.Summarize to get summary of all answers
      - you can use this file as a knowledge base or for further analysis