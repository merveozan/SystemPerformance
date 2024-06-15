# System Monitor Application

## Overview
This application monitors the system's CPU and RAM usage and alerts the user via Telegram and email if certain thresholds are exceeded.

## Setup Instructions

### Prerequisites
- .NET Core SDK
- Environment variables setup

### Variables
You need to set up the following variables in code:
- `botToken`: Your Telegram bot token.
- `chatId`: Your Telegram chat ID.
- `smtpUser`: Your SMTP email address.
- `smtpPassword`: Your SMTP password.

### Obtaining the `BOT_TOKEN`
1. Talk to the [BotFather](https://t.me/botfather) on Telegram to create a new bot.
2. BotFather will provide you with a token. Use this as your `BOT_TOKEN`.
   ![Bot Token Setup](https://github.com/merveozan/SystemPerformance/blob/main/bot_token_setup.png "Bot Token Setup")

### Obtaining the `CHAT_ID`
1. Send a message to your bot.
2. Use any Telegram API development tool to retrieve the `chat_id` from your message.
   ![Chat ID Retrieval](https://github.com/merveozan/SystemPerformance/blob/main/chat_id_retrieval.png "Chat ID Retrieval")

### Running the Application
Run the application from the command line:
```bash
dotnet run

