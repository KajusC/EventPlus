import React from 'react';

function EventCard({ event }) {
    return (
        <div className="event-card">
            <h2>{event.name}</h2>
            <p>{event.description}</p>
            <p>Category: {event.category}</p>
            <p>Start Date: {event.startDate}</p>
            <p>End Date: {event.endDate}</p>
            <p>Tickets Available: {event.ticketsAvailable}</p>
            <button>View Details</button>
        </div>
    );
}

export default EventCard;