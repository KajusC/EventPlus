import { Link, useNavigate } from 'react-router-dom';
import './App.css';

function EventList() {
    // Hardcodinti eventai
    const events = [
/*        { id: 1, name: 'Concert in the Wild', ticketsAvailable: 100 },*/
        { id: 1, name: 'Art Gallery Opening', ticketsAvailable: 50 },
        { id: 2, name: 'Tech Conference 2025', ticketsAvailable: 200 },
        { id: 3, name: 'Theater Play: Hamlet', ticketsAvailable: 80 },
        { id: 4, name: 'Lady Gaga Kaunas', ticketsAvailable: 1000}
    ];

    const navigate = useNavigate();

    return (
        <div>
            <h1 id="tableLabel">Event List</h1>
            <table>
                <thead>
                    <tr>
                        <th>Event Name</th>
                        <th>Tickets Available</th>
                    </tr>
                </thead>

                <tbody>
                    {events.map(event => (
                        <tr key={event.id}>
                            <td>{event.name}</td>
                            <td>{event.ticketsAvailable}</td>
                            <td>
                                <Link to={`/eventview/${event.id}`}>
                                    <button>View</button>
                                </Link>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <Link to={`/eventinsert`}>
                <button>Insert</button>
            </Link>
            <span style={{ margin: '0 10px' }}></span>
            <button onClick={() => navigate(-1)}>Return</button>
        </div>
    );
}

export default EventList;
