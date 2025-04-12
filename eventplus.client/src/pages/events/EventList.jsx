import React from 'react';
import {
    Grid,
    Container,
    Typography,
    Box
} from '@mui/material';
import EventCard from '../../components/events/EventCard';
import { fetchEvents } from '../../services/eventService'; // Adjust the import based on your file structure

function EventList() {

    const [events, setEvents] = React.useState([[]]);

    React.useEffect(() => {

        const getEvents = async () => {
            try {
                const data = await fetchEvents();
                setEvents(data);
            } catch (error) {
                console.error("Error fetching events:", error);
            }
        };

        getEvents();
    }, []);
    

    return (
        <Box
            display="flex"
            flexDirection="column"
            alignItems="center"
            justifyContent="flex-start"
            sx={{
                minHeight: '100vh',
                padding: '100px',
            }}
        >
            <Container maxWidth="md">
                <Typography variant="h4" align="center" gutterBottom>
                    Upcoming Events
                </Typography>
                <Grid container spacing={3} justifyContent="center">
                    {events.map((event) => (
                        <Grid key={event.idEvent} item xs={12} sm={6} md={4}>
                            <EventCard event={event} />
                        </Grid>
                    ))}
                </Grid>
            </Container>
        </Box>
    );
}

export default EventList;