import React from 'react';

function EventList() {
    // Sample data for events
    const events = [
        {
            id: 1,
            name: 'Concert in the Wild',
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
            description: 'A live performance of Shakespeare\'s classic play, Hamlet.',
            category: 'Theater',
            startDate: '2025-08-01 18:00',
            endDate: '2025-08-01 22:00',
            ticketsAvailable: 80
        }
    ];

    return (
        <div>
            <h1>Event List</h1>
            <ul>
                {events.map(event => (
                    <li key={event.id}>
                        <h2>{event.name}</h2>
                        <p>{event.description}</p>
                        <p>Category: {event.category}</p>
                        <p>Start Date: {event.startDate}</p>
                        <p>End Date: {event.endDate}</p>
                        <p>Tickets Available: {event.ticketsAvailable}</p>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default EventList;