# LexAppFinalProject
Repository for my final project application developed as part of my training in programming and AI at Lexicon.

This ASP.NET Core MVC application stores movies and TV shows found via Open Movie Database's API in a local database.

## How to use
NOTE: Incognito mode is recommended to prevent saving your browser history with the application.

0. **THE BEFORE-YOU-USE!…** Open the project in Virtual Studio, and run the "update-database" command in the Package Manager Console to create the database and tables. (If you don't do this, the application will not work!)
1. Click on either button to go to respective media type's index, featuring a table where information about your saved movies/TV shows is displayed
2. Add a movie or TV show by clicking the respective button in the upper-left corner, above the media table
3. From the menu for adding an item, if you have an API key, you can input it and the approximate (or exact) name of what you are looking for.
4. Your query to Open Movie Database will try to find an item with your search string, then add it if anything was found. On success, you will be returned to the index.
