//import { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import EventList from './EventList';
import EventView from './EventView';
import EventEdit from './EventEdit';
import EventInsert from './EventInsert';
import './App.css';

function WeatherForecast() {
    //const [forecasts, setForecasts] = useState();
    const navigate = useNavigate();

  /*  useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;*/

    return (
        <div>
            <h1 id="tableLabel">EventPlus</h1>
            {/*{contents}*/}
            <button onClick={() => navigate('/events')}>Go to Event List</button>
        </div>
    );

/*    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        setForecasts(data);
    }*/
}

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<WeatherForecast />} />
                <Route path="/events" element={<EventList />} />
                <Route path="/eventview/:id" element={<EventView />} />
                <Route path="/eventedit/:id" element={<EventEdit />} />
                <Route path="/eventinsert" element={<EventInsert />} />
            </Routes>
        </Router>
    );
}

export default App;
