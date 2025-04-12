import React from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchEventById } from '../../services/eventService';
import {
    Container,
    Typography,
    Button,
    Paper,
    Box,
    Chip,
    CircularProgress,
    Alert,
    Grid,
    Divider,
    Card,
    CardContent,
    Stack
} from '@mui/material';
import {
    CalendarMonth as CalendarIcon,
    Category as CategoryIcon,
    Description as DescriptionIcon,
    ConfirmationNumber as TicketIcon
} from '@mui/icons-material';

const styles = {
    card: { 
        borderRadius: 4, 
        boxShadow: '0 8px 40px -12px rgba(0,0,0,0.2)',
        overflow: 'hidden',
    },
    box: { 
        height: '180px', 
        bgcolor: '#030027', 
        position: 'relative',
        display: 'flex',
        alignItems: 'flex-end'
    },
    typography1: { 
        color: 'white', 
        p: 3, 
        pb: 2,
        textShadow: '1px 1px 3px rgba(0,0,0,0.3)',
        fontWeight: 'bold' 
    },
    chip: { 
        position: 'absolute', 
        top: 20, 
        right: 20,
        fontWeight: 'bold'
    }
}

function EventView() {
    const { id } = useParams();
    const navigate = useNavigate();

    const [event, setEvent] = React.useState(null);
    const [loading, setLoading] = React.useState(true);
    const [error, setError] = React.useState(null);

    React.useEffect(() => {
        const fetchEvent = async () => {
            try {
                const data = await fetchEventById(id);
                setEvent(data);
            } catch (err) {
                setError(err.message || 'Failed to load event');
            } finally {
                setLoading(false);
            }
        }
        fetchEvent();
    }, [id]);

    const handleDelete = () => {
        const userConfirmed = window.confirm("Are you sure you want to delete this event?");
        if (userConfirmed) {
            alert("Event deleted successfully.");
            navigate('/events');
        } else {
            alert("Event deletion canceled.");
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Container maxWidth="md" sx={{ mt: 4 }}>
                <Alert severity="error">{error}</Alert>
            </Container>
        );
    }

    if (!event) {
        return (
            <Container maxWidth="md" sx={{ mt: 4 }}>
                <Alert severity="info">Event not found</Alert>
            </Container>
        );
    }

    function formatDate(dateString) {
        const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    }

    return (
        <Container maxWidth="md" sx={{ mt: 8, mb: 4, minHeight: '80vh' }}>
            <Card sx={styles.card}>
                <Box sx={styles.box}>
                    <Typography 
                        variant="h3" 
                        sx={styles.typography1}
                    >
                        {event.name}
                    </Typography>
                    <Chip 
                        label={event.category} 
                        color="secondary" 
                        size="small"
                        sx={styles.chip} 
                    />
                </Box>
                
                <CardContent sx={{ py: 4 }}>
                    <Grid container spacing={4}>
                        <Grid item xs={12}>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <DescriptionIcon sx={{ mr: 2, color: 'text.secondary' }} />
                                <Typography variant="body1">
                                    {event.description}
                                </Typography>
                            </Box>
                        </Grid>
                        
                        <Grid item xs={12}>
                            <Divider />
                        </Grid>
                        
                        <Grid item xs={12} sm={6}>
                            <Stack spacing={3}>
                                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                    <CalendarIcon sx={{ mr: 2, color: 'text.secondary' }} />
                                    <Box>
                                        <Typography variant="subtitle2" color="text.secondary">
                                            Start Date
                                        </Typography>
                                        <Typography variant="body1" fontWeight="medium">
                                            {formatDate(event.startDate)}
                                        </Typography>
                                    </Box>
                                </Box>
                                
                                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                    <CalendarIcon sx={{ mr: 2, color: 'text.secondary' }} />
                                    <Box>
                                        <Typography variant="subtitle2" color="text.secondary">
                                            End Date
                                        </Typography>
                                        <Typography variant="body1" fontWeight="medium">
                                            {formatDate(event.endDate)}
                                        </Typography>
                                    </Box>
                                </Box>
                            </Stack>
                        </Grid>
                        
                        <Grid item xs={12} sm={6}>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <TicketIcon sx={{ mr: 2, color: 'text.secondary' }} />
                                <Box>
                                    <Typography variant="subtitle2" color="text.secondary">
                                        Tickets Available
                                    </Typography>
                                    <Typography variant="h4" fontWeight="bold" color="primary.main">
                                        {event.ticketsAvailable}
                                    </Typography>
                                </Box>
                            </Box>
                        </Grid>
                    </Grid>
                    
                    <Box mt={5} pt={3} sx={{ borderTop: '1px solid', borderColor: 'divider' }}>
                        <Grid container spacing={2} justifyContent="space-between">
                            <Grid item>
                                <Button 
                                    variant="contained" 
                                    size="large"
                                    color="secondary" 
                                    sx={{ px: 4, borderRadius: 2, fontWeight: 'bold' }}
                                >
                                    Purchase Tickets
                                </Button>
                            </Grid>
                            <Grid item>
                                <Stack direction="row" spacing={2}>
                                    <Button 
                                        component={Link} 
                                        to={`/eventedit/${event.idEvent}`} 
                                        variant="outlined"
                                        size="medium"
                                    >
                                        Edit
                                    </Button>
                                    <Button 
                                        onClick={handleDelete} 
                                        variant="outlined" 
                                        color="error"
                                        size="medium"
                                    >
                                        Delete
                                    </Button>
                                    <Button 
                                        onClick={() => navigate(-1)} 
                                        variant="text"
                                        size="medium"
                                    >
                                        Back
                                    </Button>
                                </Stack>
                            </Grid>
                        </Grid>
                    </Box>
                </CardContent>
            </Card>
        </Container>
    );
}

export default EventView;