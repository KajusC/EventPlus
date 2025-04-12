import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

function EventInsert() {
    const navigate = useNavigate();

    // Inicializuojame state su tuščiais duomenimis
    const [eventData, setEventData] = useState({
        name: '',
        description: '',
        category: '',
        startDate: '',
        endDate: '',
        ticketsAvailable: 0
    });

    // Funkcija, kuri keičia formos laukų reikšmes
    const handleChange = (e) => {
        const { name, value } = e.target;
        setEventData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Funkcija, kuri išsaugo įvykį
    const handleSave = () => {
        // Patikriname, ar privalomi laukai užpildyti
        if (!eventData.startDate) {
            alert("Start Date cannot be null.");
            return;
        }

        // Čia galėtų būti kodas, kuris išsaugo įvykį į serverį ar vietinį duomenų saugojimą
        alert("Event saved successfully!");
        //navigate('/events'); // Nukreipiame atgal į įvykių sąrašą
    };

    return (
        <div>
            <h1>Create New Event</h1>
            <form>
                <div>
                    <label>Name:</label>
                    <input
                        type="text"
                        name="name"
                        value={eventData.name}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label>Description:</label>
                    <input
                        type="text"
                        name="description"
                        value={eventData.description}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label>Category:</label>
                    <input
                        type="text"
                        name="category"
                        value={eventData.category}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label>Start Date:</label>
                    <input
                        type="text"
                        name="startDate"
                        value={eventData.startDate}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label>End Date:</label>
                    <input
                        type="text"
                        name="endDate"
                        value={eventData.endDate}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label>Tickets Available:</label>
                    <input
                        type="number"
                        name="ticketsAvailable"
                        value={eventData.ticketsAvailable}
                        onChange={handleChange}
                    />
                </div>
            </form>
            <button onClick={handleSave}>Save</button>
            <button onClick={() => navigate(-1)}>Return</button>
        </div>
    );
}

export default EventInsert;
