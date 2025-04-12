import React, { useState } from 'react';
import EventForm from '../../components/events/EventForm';
import { useNavigate } from 'react-router-dom';
import eventService from '../../services/eventService';

function EventInsert() {
    const [eventData, setEventData] = useState({
        name: '',
        description: '',
        category: '',
        startDate: '',
        endDate: '',
        ticketsAvailable: 0
    });

    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setEventData({
            ...eventData,
            [name]: value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await eventService.createEvent(eventData);
            alert("Event created successfully.");
            navigate('/events'); // Redirect to the event list after creation
        } catch (error) {
            alert("Error creating event: " + error.message);
        }
    };

    return (
        <div>
            <h1>Create New Event</h1>
            <EventForm 
                eventData={eventData} 
                onChange={handleChange} 
                onSubmit={handleSubmit} 
            />
        </div>
    );
}

export default EventInsert;