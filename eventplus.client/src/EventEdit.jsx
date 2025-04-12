import { useParams, useNavigate } from 'react-router-dom';
import { useState } from 'react';

function EventEdit() {
    const { id } = useParams();
    const navigate = useNavigate();

    // Hardcodinti eventai
    const events = [
        {
            id: 1,
            name: 'Concert in the Park',
            description: 'An open-air concert in the park featuring local artists.',
            category: 'Music',
            startDate: '2025-05-01 19:00',
            endDate: '2025-05-01 22:00',
            ticketsAvailable: 100
        },
        {
            id: 2,
            name: 'Art Gallery Opening',
            description: 'An exclusive opening of a new art gallery showcasing contemporary artists.',
            category: 'Art',
            startDate: '2025-06-10 14:00',
            endDate: '2025-06-12 16:00',
            ticketsAvailable: 50
        },
        {
            id: 3,
            name: 'Tech Conference 2025',
            description: 'A leading technology conference with the latest innovations and trends.',
            category: 'Technology',
            startDate: '2025-07-15 12:00',
            endDate: '2025-07-17 18:00',
            ticketsAvailable: 200
        },
        {
            id: 4,
            name: 'Theater Play: Hamlet',
            description: 'A live performance of Shakespeare’s classic play, Hamlet.',
            category: 'Theater',
            startDate: '2025-08-01 18:00',
            endDate: '2025-08-01 22:00',
            ticketsAvailable: 80
        }
    ];

    const event = events.find(e => e.id === parseInt(id));
    // State sukurimas, kad būtų galima redaguoti įvedamus duomenis
    const [eventData, setEventData] = useState({
        name: event.name,
        description: event.description,
        category: event.category,
        startDate: event.startDate,
        endDate: event.endDate,
        ticketsAvailable: event.ticketsAvailable
    });

    if (!event) {
        return <div>Event not found</div>;
    }


    // Funkcija, kuri keičia duomenis state
    const handleChange = (e) => {
        const { name, value } = e.target;
        setEventData(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSave = () => {
        // Čia galėtų būti kodas, kuris išsaugo duomenis į serverį ar panašiai
        alert("Start Date cannot be null.");
    };

    return (
        <div>
            <h1>Edit Event</h1>
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

export default EventEdit;
