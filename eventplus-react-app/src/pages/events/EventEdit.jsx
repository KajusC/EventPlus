import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import EventForm from '../../components/events/EventForm';
import eventService from '../../services/eventService';

function EventEdit() {
    const { id } = useParams();
    const navigate = useNavigate();

    const handleSubmit = async (eventData) => {
        try {
            await eventService.updateEvent(id, eventData);
            alert("Event updated successfully.");
            navigate(`/events/${id}`);
        } catch (error) {
            alert("Error updating event: " + error.message);
        }
    };

    return (
        <div>
            <h1>Edit Event</h1>
            <EventForm onSubmit={handleSubmit} eventId={id} />
        </div>
    );
}

export default EventEdit;