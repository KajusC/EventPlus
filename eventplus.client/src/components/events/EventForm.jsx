import React, { useState } from 'react';

function EventForm({ event, onSubmit }) {
    const [name, setName] = useState(event ? event.name : '');
    const [description, setDescription] = useState(event ? event.description : '');
    const [category, setCategory] = useState(event ? event.category : '');
    const [startDate, setStartDate] = useState(event ? event.startDate : '');
    const [endDate, setEndDate] = useState(event ? event.endDate : '');
    const [ticketsAvailable, setTicketsAvailable] = useState(event ? event.ticketsAvailable : '');

    const handleSubmit = (e) => {
        e.preventDefault();
        const eventData = {
            name,
            description,
            category,
            startDate,
            endDate,
            ticketsAvailable
        };
        onSubmit(eventData);
    };

    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label>Name:</label>
                <input type="text" value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div>
                <label>Description:</label>
                <textarea value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>
            <div>
                <label>Category:</label>
                <input type="text" value={category} onChange={(e) => setCategory(e.target.value)} required />
            </div>
            <div>
                <label>Start Date:</label>
                <input type="datetime-local" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
            </div>
            <div>
                <label>End Date:</label>
                <input type="datetime-local" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
            </div>
            <div>
                <label>Tickets Available:</label>
                <input type="number" value={ticketsAvailable} onChange={(e) => setTicketsAvailable(e.target.value)} required />
            </div>
            <button type="submit">{event ? 'Update Event' : 'Create Event'}</button>
        </form>
    );
}

export default EventForm;