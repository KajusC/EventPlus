# EventPlus React Application

## Overview
EventPlus is a React application designed to manage events. It allows users to view, create, edit, and delete events, providing a user-friendly interface for event management.

## Project Structure
The project is organized into the following directories:

- **src**: Contains all the source code for the application.
  - **components**: Contains reusable components.
    - **common**: Common components used throughout the application (Header, Footer, Navigation).
    - **events**: Components specific to event management (EventCard, EventForm, EventFilters).
  - **pages**: Contains the main pages of the application.
    - **MainMenu**: The main menu page linking to different sections.
    - **events**: Pages related to event management (EventView, EventList, EventInsert, EventEdit).
  - **services**: Contains services for handling API calls related to events.
  - **utils**: Utility functions for formatting data.
  - **App.jsx**: The main application component that sets up routing.
  - **index.jsx**: The entry point of the application.
  - **routes.js**: Defines the routing configuration.

## Installation
To get started with the EventPlus application, follow these steps:

1. Clone the repository:
   ```
   git clone <repository-url>
   ```

2. Navigate to the project directory:
   ```
   cd eventplus-react-app
   ```

3. Install the dependencies:
   ```
   npm install
   ```

## Running the Application
To run the application in development mode, use the following command:
```
npm start
```
This will start the application and open it in your default web browser.

## Features
- View detailed information about events.
- List all available events.
- Insert new events using a form.
- Edit existing events.
- Delete events with confirmation.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.