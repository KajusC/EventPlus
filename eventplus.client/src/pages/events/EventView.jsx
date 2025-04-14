import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchEventById, deleteEvent } from '../../services/eventService';
import {
    Container,
    Typography,
    Button,
    Box,
    Chip,
    CircularProgress,
    Alert,
    Grid,
    Divider,
    Card,
    CardContent,
    Stack,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
    Snackbar,
    IconButton
} from '@mui/material';
import {
    CalendarMonth as CalendarIcon,
    LocationOn as LocationIcon,
    Category as CategoryIcon,
    Description as DescriptionIcon,
    ConfirmationNumber as TicketIcon,
    Edit as EditIcon,
    DeleteOutline as DeleteIcon,
    ArrowBack as ArrowBackIcon,
    Share as ShareIcon
} from '@mui/icons-material';

// Category name mapping
const getCategoryName = (categoryId) => {
    const categories = {
        1: 'Music',
        2: 'Business',
        // Add more mappings as needed
        default: 'Event'
    };
    return categories[categoryId] || categories.default;
};

// Get category color
const getCategoryColor = (categoryId) => {
    const colors = {
        1: '#6a11cb', // Music - our primary purple
        2: '#2575fc', // Business - our primary blue
        default: '#6a11cb'
    };
    return colors[categoryId] || colors.default;
};

// Event category to image mapping
const getCategoryImage = (category) => {
    const images = {
        1: 'https://source.unsplash.com/random/1200x500/?concert',
        2: 'https://source.unsplash.com/random/1200x500/?conference',
        // Add more mappings as needed
        default: 'https://source.unsplash.com/random/1200x500/?event'
    };
    return images[category] || images.default;
};

function EventView() {
    const { id } = useParams();
    const navigate = useNavigate();
    
    const [event, setEvent] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [deleteLoading, setDeleteLoading] = useState(false);
    const [toast, setToast] = useState({ open: false, message: '', severity: 'info' });

    useEffect(() => {
        const fetchEvent = async () => {
            try {
                setLoading(true);
                const data = await fetchEventById(id);
                setEvent(data);
                setError(null);
            } catch (err) {
                setError(err.message || 'Failed to load event');
            } finally {
                setLoading(false);
            }
        };
        
        fetchEvent();
    }, [id]);

    const handleDeleteClick = () => {
        setDeleteDialogOpen(true);
    };

    const handleDeleteCancel = () => {
        setDeleteDialogOpen(false);
    };

    const handleDeleteConfirm = async () => {
        try {
            setDeleteLoading(true);
            await deleteEvent(id);
            setToast({
                open: true,
                message: 'Event deleted successfully.',
                severity: 'success'
            });
            setTimeout(() => {
                navigate('/events');
            }, 1500);
        } catch (err) {
            setError(err.message || 'Failed to delete event');
            setToast({
                open: true,
                message: `Error: ${err.message || 'Failed to delete event'}`,
                severity: 'error'
            });
        } finally {
            setDeleteLoading(false);
            setDeleteDialogOpen(false);
        }
    };

    const handleCloseToast = (event, reason) => {
        if (reason === 'clickaway') return;
        setToast({ ...toast, open: false });
    };

    function formatDate(dateString) {
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    }

    function formatTime(dateString) {
        const options = { hour: '2-digit', minute: '2-digit' };
        return new Date(dateString).toLocaleTimeString(undefined, options);
    }

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
                <CircularProgress sx={{ color: '#6a11cb' }} />
            </Box>
        );
    }

    if (error) {
        return (
            <Container maxWidth="md" sx={{ mt: 12, mb: 4 }}>
                <Alert severity="error">{error}</Alert>
            </Container>
        );
    }

    if (!event) {
        return (
            <Container maxWidth="md" sx={{ mt: 12, mb: 4 }}>
                <Alert severity="info">Event not found</Alert>
            </Container>
        );
    }

    const categoryName = getCategoryName(event.category);
    const categoryColor = getCategoryColor(event.category);
    const imageUrl = getCategoryImage(event.category);

    return (
        <>
            <Box 
                sx={{
                    width: '100%',
                    position: 'relative',
                    height: { xs: '300px', md: '400px' },
                    overflow: 'hidden',
                    mt: 8,
                }}
            >
                <Box
                    sx={{
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        width: '100%',
                        height: '100%',
                        backgroundImage: `url(${imageUrl})`,
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

                    <Box sx={{ display: 'flex', alignItems: 'center', mt: 2 }}>
                        <CalendarIcon sx={{ color: 'white', mr: 1, fontSize: 20 }} />
                        <Typography variant="subtitle1" sx={{ color: 'white', mr: 3 }}>
                            {formatDate(event.startDate)} at {formatTime(event.startDate)}
                        </Typography>
                    </Box>
                </Container>
            </Box>

            <Container maxWidth="lg" sx={{ mt: { xs: -3, md: -5 }, mb: 8, position: 'relative', zIndex: 3 }}>
                <Grid container spacing={4}>
                    {/* Main Content */}
                    <Grid item xs={12} md={8}>
                        <Card sx={{ 
                            borderRadius: 3, 
                            overflow: 'hidden', 
                            boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                        }}>
                            <CardContent sx={{ p: 4 }}>
                                <Box sx={{ mb: 4 }}>
                                    <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 600 }}>
                                        About This Event
                                    </Typography>
                                    <Typography variant="body1" color="text.secondary" sx={{ whiteSpace: 'pre-line', lineHeight: 1.8 }}>
                                        {event.description}
                                    </Typography>
                                </Box>

                                <Divider sx={{ my: 4 }} />

                                <Box>
                                    <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 600 }}>
                                        Event Details
                                    </Typography>
                                    
                                    <Grid container spacing={3}>
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

                        <Box sx={{ mt: 4, display: 'flex', justifyContent: 'space-between' }}>
                            <Button 
                                variant="outlined"
                                color="inherit"
                                startIcon={<ArrowBackIcon />}
                                onClick={() => navigate('/events')}
                                sx={{ 
                                    borderColor: 'rgba(250, 250, 250, 0.23)',
                                    color: 'white',
                                    '&:hover': {
                                        bgcolor: 'rgba(0,0,0,0.04)',
                                        borderColor: 'text.primary'
                                    }
                                }}
                            >
                                Back to Events
                            </Button>

                            <Box>
                                <IconButton 
                                    aria-label="share event"
                                    sx={{ 
                                        mr: 1,
                                        color: 'text.secondary',
                                        '&:hover': { color: '#2575fc' }
                                    }}
                                >
                                    <ShareIcon />
                                </IconButton>
                                <Button
                                    component={Link}
                                    to={`/eventedit/${event.idEvent}`}
                                    variant="outlined"
                                    color="primary"
                                    startIcon={<EditIcon />}
                                    sx={{ 
                                        mr: 1,
                                        borderColor: '#6a11cb',
                                        color: '#6a11cb',
                                        '&:hover': {
                                            borderColor: '#2575fc',
                                            bgcolor: 'rgba(106, 17, 203, 0.04)'
                                        }
                                    }}
                                >
                                    Edit
                                </Button>
                                <Button
                                    variant="outlined"
                                    color="error"
                                    startIcon={<DeleteIcon />}
                                    onClick={handleDeleteClick}
                                >
                                    Delete
                                </Button>
                            </Box>
                        </Box>
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

            {/* Delete Dialog */}
            <Dialog
                open={deleteDialogOpen}
                onClose={handleDeleteCancel}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">
                    {"Confirm Event Deletion"}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        Are you sure you want to delete this event? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
                <DialogActions sx={{ px: 3, pb: 3 }}>
                    <Button 
                        onClick={handleDeleteCancel} 
                        color="inherit"
                        variant="outlined"
                        disabled={deleteLoading}
                    >
                        Cancel
                    </Button>
                    <Button 
                        onClick={handleDeleteConfirm} 
                        color="error" 
                        variant="contained"
                        autoFocus
                        disabled={deleteLoading}
                        startIcon={deleteLoading ? <CircularProgress size={16} color="inherit" /> : null}
                    >
                        {deleteLoading ? 'Deleting...' : 'Delete Event'}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Snackbar for notifications */}
            <Snackbar
                open={toast.open}
                autoHideDuration={3000}
                onClose={handleCloseToast}
                anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
            >
                <Alert 
                    onClose={handleCloseToast} 
                    severity={toast.severity} 
                    sx={{ width: '100%' }}
                    elevation={6}
                    variant="filled"
                >
                    {toast.message}
                </Alert>
            </Snackbar>
        </>
    );
}

export default EventView;