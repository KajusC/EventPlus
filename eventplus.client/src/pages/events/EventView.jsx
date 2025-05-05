import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchEventById, deleteEvent } from '../../services/eventService';
import { fetchCategories } from '../../services/categoryService';
import {
    Container,
    Typography,
    Button,
    Box,
    Chip,
    Grid,
    Divider,
    Card,
    CardContent,
    IconButton
} from '@mui/material';
import {
    CalendarMonth as CalendarIcon,
    LocationOn as LocationIcon,
    Category as CategoryIcon,
    ConfirmationNumber as TicketIcon,
    ArrowBack as ArrowBackIcon,
    DeleteOutline as DeleteIcon,
    Edit as EditIcon
} from '@mui/icons-material';

// Import shared components
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';
import ToastNotification from '../../components/shared/ToastNotification';
import ConfirmationDialog from '../../components/shared/ConfirmationDialog';

// Import utilities
import { formatDate, formatTime } from '../../utils/dateFormatter';

const getCategoryColor = (categoryId) => {
    const colors = {
        1: '#6a11cb',
        2: '#2575fc',
        default: '#6a11cb'
    };
    return colors[categoryId] || colors.default;
};

function EventView() {
    const { id } = useParams();
    const navigate = useNavigate();
    
    // Simple state - only what this component needs to track
    const [event, setEvent] = useState(null);
    const [categories, setCategories] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    
    // UI interaction states
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);
    const [toast, setToast] = useState({
        open: false,
        message: '',
        severity: 'info'
    });

    // Load categories
    useEffect(() => {
        fetchCategories()
            .then(data => setCategories(data))
            .catch(err => console.error("Error fetching categories:", err));
    }, []);

    // Load event data
    useEffect(() => {
        setIsLoading(true);
        fetchEventById(id)
            .then(data => {
                setEvent(data);
                setError(null);
            })
            .catch(err => {
                setError(err.message || 'Failed to load event');
            })
            .finally(() => {
                setIsLoading(false);
            });
    }, [id]);

    const getCategoryName = (categoryId) => {
        if (!categories || categories.length === 0) return "Event";
        const category = categories.find(cat => cat.idCategory === categoryId);
        return category ? category.name : "Event";
    };

    // Delete event handler
    const handleDeleteEvent = async () => {
        setIsDeleting(true);
        try {
            await deleteEvent(id);
            setToast({
                open: true,
                message: 'Event deleted successfully',
                severity: 'success'
            });
            // Redirect after short delay to show the success message
            setTimeout(() => navigate('/events'), 1500);
        } catch (err) {
            setToast({
                open: true,
                message: `Error: ${err.message || 'Failed to delete event'}`,
                severity: 'error'
            });
        } finally {
            setIsDeleting(false);
            setShowDeleteDialog(false);
        }
    };

    // Early returns for loading and error states
    if (isLoading) return <LoadingSpinner />;
    if (error) return <ErrorDisplay error={error} />;
    if (!event) return <ErrorDisplay error="Event not found" />;

    // Event found, display the data
    const categoryName = getCategoryName(event.category);
    const categoryColor = getCategoryColor(event.category);

    return (
        <>
            {/* Event Header/Banner */}
            <Box 
                sx={{
                    width: '100%',
                    position: 'relative',
                    height: { xs: '300px', md: '400px' },
                    overflow: 'hidden',
                    mt: 8
                }}
            >
                <Box
                    sx={{
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        width: '100%',
                        height: '100%',
                        backgroundSize: 'cover',
                        backgroundPosition: 'center',
                        '&::before': {
                            content: '""',
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            width: '100%',
                            height: '100%',
                            background: 'linear-gradient(to bottom, rgba(0,0,0,0.4) 0%, rgba(0,0,0,0.7) 100%)',
                            zIndex: 1
                        }
                    }}
                />
                <Container 
                    maxWidth="lg"
                    sx={{
                        position: 'relative',
                        zIndex: 2,
                        height: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-end',
                        pb: 4
                    }}
                >
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                        <IconButton 
                            onClick={() => navigate('/events')} 
                            sx={{ 
                                color: 'white', 
                                mr: 2,
                                bgcolor: 'rgba(255,255,255,0.1)',
                                '&:hover': {
                                    bgcolor: 'rgba(255,255,255,0.2)'
                                }
                            }}
                        >
                            <ArrowBackIcon />
                        </IconButton>
                        <Chip 
                            label={categoryName} 
                            sx={{ 
                                color: 'white',
                                bgcolor: 'rgba(106,17,203,0.8)', 
                                fontWeight: 600,
                                px: 1
                            }} 
                        />
                    </Box>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', width: '100%' }}>
                        <Typography 
                            variant="h2" 
                            component="h1"
                            sx={{
                                color: 'white',
                                fontWeight: 800,
                                textShadow: '0 2px 4px rgba(0,0,0,0.3)',
                                fontSize: { xs: '2rem', sm: '2.5rem', md: '3rem' },
                                lineHeight: 1.2
                            }}
                        >
                            {event.name}
                        </Typography>
                        
                        <Box sx={{ display: 'flex', gap: 1 }}>
                            <IconButton
                                onClick={() => navigate(`/eventedit/${id}`)}
                                sx={{ 
                                    color: 'white',
                                    bgcolor: 'rgba(255,255,255,0.1)',
                                    '&:hover': { bgcolor: 'rgba(255,255,255,0.2)' }
                                }}
                            >
                                <EditIcon />
                            </IconButton>
                            <IconButton
                                onClick={() => setShowDeleteDialog(true)}
                                sx={{ 
                                    color: 'white',
                                    bgcolor: 'rgba(255,255,255,0.1)',
                                    '&:hover': { bgcolor: 'rgba(255,255,255,0.2)' }
                                }}
                            >
                                <DeleteIcon />
                            </IconButton>
                        </Box>
                    </Box>

                    <Box sx={{ display: 'flex', alignItems: 'center', mt: 2 }}>
                        <CalendarIcon sx={{ color: 'white', mr: 1, fontSize: 20 }} />
                        <Typography variant="subtitle1" sx={{ color: 'white', mr: 3 }}>
                            {formatDate(event.startDate)} at {formatTime(event.startDate)}
                        </Typography>
                    </Box>
                </Container>
            </Box>

            {/* Event Details Content */}
            <Container maxWidth="lg" sx={{ mt: { xs: -3, md: -5 }, mb: 8, position: 'relative', zIndex: 3, pt: 3 }}>
                <Grid container spacing={4}>
                    {/* Main Content */}
                    <Grid item xs={12} md={8}>
                        <Card sx={{ 
                            borderRadius: 3, 
                            overflow: 'hidden', 
                            boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                        }}>
                            <CardContent sx={{ p: 4 }}>
                                {/* About This Event section */}
                                <Box sx={{ mb: 4 }}>
                                    <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 600 }}>
                                        About This Event
                                    </Typography>
                                    <Typography variant="body1" color="text.secondary" sx={{ whiteSpace: 'pre-line', lineHeight: 1.8 }}>
                                        {event.description}
                                    </Typography>
                                </Box>

                                <Divider sx={{ my: 4 }} />

                                {/* Event Details section */}
                                <Box>
                                    <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 600 }}>
                                        Event Details
                                    </Typography>
                                    
                                    <Grid container spacing={3}>
                                        {/* Start Date & Time */}
                                        <Grid item xs={12} sm={6}>
                                            <Box sx={{ display: 'flex', alignItems: 'flex-start', mb: 3 }}>
                                                <CalendarIcon sx={{ 
                                                    color: 'text.secondary',
                                                    mr: 2,
                                                    mt: 0.5
                                                }} />
                                                <Box>
                                                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                                        Start Date & Time
                                                    </Typography>
                                                    <Typography variant="body1" fontWeight={500}>
                                                        {formatDate(event.startDate)}
                                                        <br />
                                                        {formatTime(event.startDate)}
                                                    </Typography>
                                                </Box>
                                            </Box>
                                        </Grid>

                                        {/* End Date & Time */}
                                        <Grid item xs={12} sm={6}>
                                            <Box sx={{ display: 'flex', alignItems: 'flex-start', mb: 3 }}>
                                                <CalendarIcon sx={{ 
                                                    color: 'text.secondary',
                                                    mr: 2,
                                                    mt: 0.5
                                                }} />
                                                <Box>
                                                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                                        End Date & Time
                                                    </Typography>
                                                    <Typography variant="body1" fontWeight={500}>
                                                        {formatDate(event.endDate)}
                                                        <br />
                                                        {formatTime(event.endDate)}
                                                    </Typography>
                                                </Box>
                                            </Box>
                                        </Grid>

                                        {/* Category */}
                                        <Grid item xs={12} sm={6}>
                                            <Box sx={{ display: 'flex', alignItems: 'flex-start' }}>
                                                <CategoryIcon sx={{ 
                                                    color: 'text.secondary',
                                                    mr: 2,
                                                    mt: 0.5
                                                }} />
                                                <Box>
                                                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                                        Category
                                                    </Typography>
                                                    <Chip 
                                                        label={categoryName} 
                                                        size="small" 
                                                        sx={{ 
                                                            bgcolor: `${categoryColor}20`,
                                                            color: categoryColor,
                                                            fontWeight: 600
                                                        }} 
                                                    />
                                                </Box>
                                            </Box>
                                        </Grid>

                                        {/* Location */}
                                        <Grid item xs={12} sm={6}>
                                            <Box sx={{ display: 'flex', alignItems: 'flex-start' }}>
                                                <LocationIcon sx={{ 
                                                    color: 'text.secondary',
                                                    mr: 2,
                                                    mt: 0.5
                                                }} />
                                                <Box>
                                                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                                        Location
                                                    </Typography>
                                                    <Typography variant="body1" fontWeight={500}>
                                                        Event Location {event.fkEventLocationidEventLocation}
                                                    </Typography>
                                                </Box>
                                            </Box>
                                        </Grid>
                                    </Grid>
                                </Box>
                            </CardContent>
                        </Card>
                    </Grid>

                    {/* Ticket Info */}
                    <Grid item xs={12} md={4}>
                        <Card sx={{ 
                            borderRadius: 3, 
                            overflow: 'hidden', 
                            boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                            position: 'sticky',
                            top: 100
                        }}>
                            <Box sx={{ 
                                bgcolor: 'rgba(106,17,203,0.03)', 
                                p: 3,
                                borderBottom: '1px solid rgba(0,0,0,0.06)'
                            }}>
                                <Typography variant="h5" sx={{ fontWeight: 600, mb: 1 }}>
                                    Tickets
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    Secure your spot at this amazing event
                                </Typography>
                            </Box>
                            
                            <CardContent sx={{ p: 3 }}>
                                <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                        <TicketIcon sx={{ color: 'text.secondary', mr: 1 }} />
                                        <Typography variant="subtitle1" fontWeight={500}>
                                            Available Tickets
                                        </Typography>
                                    </Box>
                                    <Typography 
                                        variant="h4" 
                                        color="primary" 
                                        fontWeight={700}
                                        sx={{ 
                                            background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                                            WebkitBackgroundClip: 'text',
                                            WebkitTextFillColor: 'transparent'
                                        }}
                                    >
                                        {event.maxTicketCount}
                                    </Typography>
                                </Box>

                                <Box sx={{ mb: 3 }}>
                                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                        Price
                                    </Typography>
                                    <Typography variant="h5" fontWeight={600}>
                                        Free
                                    </Typography>
                                </Box>

                                <Button
                                    variant="contained"
                                    fullWidth
                                    size="large"
                                    sx={{
                                        py: 1.5,
                                        fontWeight: 600,
                                        fontSize: '1rem',
                                        borderRadius: 2,
                                        textTransform: 'none',
                                        background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                                        boxShadow: '0 5px 15px rgba(106, 17, 203, 0.3)',
                                        '&:hover': {
                                            boxShadow: '0 8px 25px rgba(106, 17, 203, 0.4)',
                                            transform: 'translateY(-2px)'
                                        },
                                        transition: 'all 0.3s ease'
                                    }}
                                >
                                    Purchase Tickets
                                </Button>
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            </Container>

            {/* Delete confirmation dialog */}
            <ConfirmationDialog
                open={showDeleteDialog}
                title="Confirm Event Deletion"
                message="Are you sure you want to delete this event? This action cannot be undone."
                confirmLabel="Delete Event"
                cancelLabel="Cancel"
                onConfirm={handleDeleteEvent}
                onCancel={() => setShowDeleteDialog(false)}
                loading={isDeleting}
                isDestructive={true}
            />

            {/* Toast notifications */}
            <ToastNotification
                open={toast.open}
                message={toast.message}
                severity={toast.severity}
                onClose={() => setToast({...toast, open: false})}
            />
        </>
    );
}

export default EventView;