import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchEventById, deleteEvent } from '../../services/eventService';
import { fetchCategories } from '../../services/categoryService';
import { fetchFeedbacksByEventId, createFeedback, updateFeedback, deleteFeedback } from '../../services/feedbackService';
import { useAuth } from '../../context/AuthContext';
import { fetchTickets } from '../../services/ticketService';
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
    IconButton,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem
} from '@mui/material';
import {
    CalendarMonth as CalendarIcon,
    LocationOn as LocationIcon,
    Category as CategoryIcon,
    ConfirmationNumber as TicketIcon,
    ArrowBack as ArrowBackIcon,
    DeleteOutline as DeleteIcon,
    Edit as EditIcon,
    Info as InfoIcon
} from '@mui/icons-material';

import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';
import ToastNotification from '../../components/shared/ToastNotification';
import ConfirmationDialog from '../../components/shared/ConfirmationDialog';
import { formatDate, formatTime } from '../../utils/dateFormatter';
import Tooltip from '@mui/material/Tooltip';
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
    const { isAdmin, isOrganizer } = useAuth();
    
    const [event, setEvent] = useState(null);
    const [categories, setCategories] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [feedbacks, setFeedbacks] = useState([]); 
    const [newFeedback, setNewFeedback] = useState({
        comment: '',
        isPositive: true,
        rating: '',
    });
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);

    const [tickets, setTickets] = useState([]);
    const [availableTickets, setAvailableTickets] = useState(0);
    const [isLoadingTickets, setIsLoadingTickets] = useState(true);

    const [editingFeedback, setEditingFeedback] = useState(null);
    const [editFeedbackData, setEditFeedbackData] = useState({ comment: '', isPositive: true, rating: '' });
    const [showFeedbackDeleteDialog, setShowFeedbackDeleteDialog] = useState(false);
    const [feedbackToDelete, setFeedbackToDelete] = useState(null);
    const [isFeedbackUpdating, setIsFeedbackUpdating] = useState(false);
    const [isFeedbackDeleting, setIsFeedbackDeleting] = useState(false);

    const [toast, setToast] = useState({
        open: false,
        message: '',
        severity: 'info'
    });

    useEffect(() => {
        fetchCategories()
            .then(data => setCategories(data))
            .catch(err => console.error("Error fetching categories:", err));
    }, []);

    useEffect(() => {
        fetchFeedbacksByEventId(id)
            .then(data => setFeedbacks(data))
            .catch(err => console.error("Error fetching feedbacks:", err));
    }, [id]);

    useEffect(() => {
        setIsLoadingTickets(true);
        if (event) {
            fetchTickets()
                .then(ticketsData => {
                    // Filter tickets for this specific event
                    const eventTickets = ticketsData.filter(ticket => 
                        ticket.fkEventidEvent === parseInt(id)
                    );
                    setTickets(eventTickets);
                    
                    // Calculate available tickets
                    const soldCount = eventTickets.length;
                    const available = Math.max(0, event.maxTicketCount - soldCount);
                    setAvailableTickets(available);
                })
                .catch(err => {
                    console.error("Error fetching tickets:", err);
                    // If we can't get tickets, assume all are available to avoid blocking sales
                    setAvailableTickets(event.maxTicketCount);
                })
                .finally(() => {
                    setIsLoadingTickets(false);
                });
        }
    }, [event, id]);

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

    const handleDeleteEvent = async () => {
        setIsDeleting(true);
        try {
            await deleteEvent(id);
            setToast({
                open: true,
                message: 'Event deleted successfully',
                severity: 'success'
            });
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
    const handleEditFeedbackClick = (feedback) => {
        setEditingFeedback(feedback.idFeedback);
        setEditFeedbackData({
            comment: feedback.comment,
            isPositive: feedback.type ? feedback.type === 1 : true,
            rating: feedback.rating,
        });
    };
    const handleFeedbackChange = (e) => {
        const { name, value } = e.target;
        setNewFeedback((prev) => ({
            ...prev,
            [name]: name === 'isPositive' ? value === 'true' : value,
        }));
    };

    const handleFeedbackSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);
        try {
            await createFeedback({
                comment: newFeedback.comment,
                type: newFeedback.isPositive ? 1 : 2,
                rating: parseFloat(newFeedback.rating),
                fkEventidEvent: id,
            });
            setToast({
                open: true,
                message: 'Feedback submitted successfully!',
                severity: 'success',
            });
            setNewFeedback({ comment: '', isPositive: true, rating: '' });
            fetchFeedbacksByEventId(id).then((data) => setFeedbacks(data));
        } catch (err) {
            setToast({
                open: true,
                message: `Error: ${err.message || 'Failed to submit feedback'}`,
                severity: 'error',
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleDeleteFeedbackClick = (feedback) => {
        setFeedbackToDelete(feedback);
        setShowFeedbackDeleteDialog(true);
    };

    const handleConfirmDeleteFeedback = async () => {
        if (!feedbackToDelete) return;
        setIsFeedbackDeleting(true);
        try {
            await deleteFeedback(feedbackToDelete.idFeedback);
            setToast({
                open: true,
                message: 'Feedback deleted successfully!',
                severity: 'success',
            });
            fetchFeedbacksByEventId(id).then((data) => setFeedbacks(data));
        } catch (err) {
            setToast({
                open: true,
                message: `Error: ${err.message || 'Failed to delete feedback'}`,
                severity: 'error',
            });
        } finally {
            setIsFeedbackDeleting(false);
            setShowFeedbackDeleteDialog(false);
            setFeedbackToDelete(null);
        }
    };

    const handleEditFeedbackChange = (e) => {
        const { name, value } = e.target;
        setEditFeedbackData((prev) => ({
            ...prev,
            [name]: name === 'isPositive' ? value === 'true' : value,
        }));
    };

    const handleEditFeedbackSubmit = async (e) => {
        e.preventDefault();
        setIsFeedbackUpdating(true);
        try {
            await updateFeedback({
                ...editFeedbackData,
                type: editFeedbackData.isPositive ? 1 : 2,
                idFeedback: editingFeedback,
                fkEventidEvent: id,
            });
            setToast({
                open: true,
                message: 'Feedback updated successfully!',
                severity: 'success',
            });
            setEditingFeedback(null);
            fetchFeedbacksByEventId(id).then((data) => setFeedbacks(data));
        } catch (err) {
            setToast({
                open: true,
                message: `Error: ${err.message || 'Failed to update feedback'}`,
                severity: 'error',
            });
        } finally {
            setIsFeedbackUpdating(false);
        }
    };


    if (isLoading) return <LoadingSpinner />;
    if (error) return <ErrorDisplay error={error} />;
    if (!event) return <ErrorDisplay error="Event not found" />;

    const categoryName = getCategoryName(event.category);
    const categoryColor = getCategoryColor(event.category);
    const isSoldOut = availableTickets <= 0;

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
                                    {isSoldOut 
                                        ? "Sorry, this event is sold out" 
                                        : "Secure your spot at this amazing event"}
                                </Typography>
                            </Box>
                            
                            <CardContent sx={{ p: 3 }}>
                                <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                        <TicketIcon sx={{ color: 'text.secondary', mr: 1 }} />
                                        <Typography variant="subtitle1" fontWeight={500}>
                                            Available Tickets
                                        </Typography>
                                        {isLoadingTickets && (
                                            <Typography 
                                                variant="caption" 
                                                color="text.secondary" 
                                                sx={{ ml: 1, fontStyle: 'italic' }}
                                            >
                                                (Loading...)
                                            </Typography>
                                        )}
                                        <Tooltip title="Tickets are claimed on a first-come, first-served basis">
                                            <InfoIcon sx={{ fontSize: 16, ml: 1, color: 'text.secondary' }} />
                                        </Tooltip>
                                    </Box>
                                    <Typography 
                                        variant="h4" 
                                        fontWeight={700}
                                        sx={{ 
                                            background: isSoldOut 
                                                ? 'linear-gradient(45deg, #ff3366 30%, #ff5555 90%)' 
                                                : 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                                            WebkitBackgroundClip: 'text',
                                            WebkitTextFillColor: 'transparent'
                                        }}
                                    >
                                        {isLoadingTickets ? "-" : availableTickets}
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

                                {isSoldOut ? (
                                    <Button
                                        disabled
                                        variant="contained"
                                        fullWidth
                                        size="large"
                                        sx={{
                                            py: 1.5,
                                            fontWeight: 600,
                                            fontSize: '1rem',
                                            borderRadius: 2,
                                            textTransform: 'none',
                                            background: 'rgba(106, 17, 203, 0.3)',
                                            boxShadow: 'none',
                                        }}
                                    >
                                        Sold Out
                                    </Button>
                                ) : (
                                    <Button
                                        onClick={() => navigate(`../tickets/TicketPurchasePage/${id}`)} 
                                        variant="contained"
                                        fullWidth
                                        size="large"
                                        disabled={isLoadingTickets}
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
                                        {isLoadingTickets ? "Loading..." : "Purchase Tickets"}
                                    </Button>
                                )}
                                
                                {/* Show a warning when tickets are low */}
                                {!isLoadingTickets && availableTickets > 0 && availableTickets < 10 && (
                                    <Typography 
                                        variant="caption" 
                                        sx={{ 
                                            display: 'block', 
                                            textAlign: 'center', 
                                            mt: 2, 
                                            color: 'warning.main',
                                            fontWeight: 500
                                        }}
                                    >
                                        Only {availableTickets} {availableTickets === 1 ? 'ticket' : 'tickets'} left! Get yours before they sell out.
                                    </Typography>
                                )}
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            </Container>

            <Container maxWidth="lg" sx={{ mt: 4 }}>
                <Card
                    sx={{
                        borderRadius: 3,
                        boxShadow: '0 8px 24px rgba(0,0,0,0.08)',
                        bgcolor: 'background.paper',
                        mb: 4,
                    }}
                >
                    <CardContent sx={{ p: { xs: 2, md: 4 } }}>
                        <Typography variant="h5" component="h2" sx={{ mb: 3, fontWeight: 600 }}>
                            Submit Your Feedback
                        </Typography>
                        <form onSubmit={handleFeedbackSubmit}>
                            <Grid container spacing={3}>
                                <Grid item xs={12}>
                                    <TextField
                                        label="Comment"
                                        name="comment"
                                        value={newFeedback.comment}
                                        onChange={handleFeedbackChange}
                                        fullWidth
                                        multiline
                                        rows={4}
                                        variant="outlined"
                                        required
                                    />
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <FormControl fullWidth>
                                        <InputLabel id="isPositive-label">Feedback Type</InputLabel>
                                        <Select
                                            labelId="isPositive-label"
                                            name="isPositive"
                                            value={newFeedback.isPositive.toString()}
                                            onChange={handleFeedbackChange}
                                            required
                                        >
                                            <MenuItem value="true">Positive</MenuItem>
                                            <MenuItem value="false">Negative</MenuItem>
                                        </Select>
                                    </FormControl>
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <TextField
                                        label="Rating (1-10)"
                                        name="rating"
                                        type="number"
                                        value={newFeedback.rating}
                                        onChange={handleFeedbackChange}
                                        fullWidth
                                        inputProps={{ min: 1, max: 10, step: 0.1 }}
                                        variant="outlined"
                                        required
                                    />
                                </Grid>
                                <Grid item xs={12}>
                                    <Button
                                        type="submit"
                                        variant="contained"
                                        color="primary"
                                        fullWidth
                                        disabled={isSubmitting || !newFeedback.comment || !newFeedback.rating}
                                        sx={{
                                            py: 1.5,
                                            fontWeight: 600,
                                            fontSize: '1rem',
                                            borderRadius: 2,
                                            textTransform: 'none',
                                        }}
                                    >
                                        {isSubmitting ? 'Submitting...' : 'Submit Feedback'}
                                    </Button>
                                </Grid>
                            </Grid>
                        </form>
                    </CardContent>
                </Card>
            </Container>

            {/* Feedbacks List */}

            <Container maxWidth="lg" sx={{ mt: 2, mb: 6 }}>
                {feedbacks.length > 0 ? (
                    <Grid container spacing={3}>
                        {feedbacks.map((feedback) => (
                            <Grid item xs={12} sm={6} md={4} key={feedback.idFeedback}>
                            <Card
                                sx={{
                                    borderRadius: 3,
                                    boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                                    overflow: 'hidden',
                                    height: '100%',
                                    display: 'flex',
                                    flexDirection: 'column',
                                }}
                            >
                                
                                <CardContent sx={{ p: 3 }}>
                                    {editingFeedback === feedback.idFeedback && (isAdmin || isOrganizer) ? (
                                        <form onSubmit={handleEditFeedbackSubmit}>
                                            <TextField
                                                label="Comment"
                                                name="comment"
                                                value={editFeedbackData.comment}
                                                onChange={handleEditFeedbackChange}
                                                fullWidth
                                                multiline
                                                rows={3}
                                                variant="outlined"
                                                required
                                                sx={{ mb: 2 }}
                                            />
                                            <FormControl fullWidth sx={{ mb: 2 }}>
                                                <InputLabel id={`isPositive-label-${feedback.idFeedback}`}>Feedback Type</InputLabel>
                                                <Select
                                                    labelId={`isPositive-label-${feedback.idFeedback}`}
                                                    name="isPositive"
                                                    value={(editFeedbackData.isPositive ?? true).toString()}
                                                    onChange={handleEditFeedbackChange}
                                                    required
                                                >
                                                    <MenuItem value="true">Positive</MenuItem>
                                                    <MenuItem value="false">Negative</MenuItem>
                                                </Select>
                                            </FormControl>
                                            <TextField
                                                label="Rating (1-10)"
                                                name="rating"
                                                type="number"
                                                value={editFeedbackData.rating}
                                                onChange={handleEditFeedbackChange}
                                                fullWidth
                                                inputProps={{ min: 1, max: 10, step: 0.1 }}
                                                variant="outlined"
                                                required
                                                sx={{ mb: 2 }}
                                            />
                                            <Box sx={{ display: 'flex', gap: 1 }}>
                                                <Button
                                                    type="submit"
                                                    variant="contained"
                                                    color="primary"
                                                    disabled={isFeedbackUpdating}
                                                >
                                                    {isFeedbackUpdating ? 'Saving...' : 'Save'}
                                                </Button>
                                                <Button
                                                    variant="outlined"
                                                    onClick={() => setEditingFeedback(null)}
                                                    disabled={isFeedbackUpdating}
                                                >
                                                    Cancel
                                                </Button>
                                            </Box>
                                        </form>
                                    ) : (
                                        <>
                                            <Typography
                                                variant="body1"
                                                color="text.primary"
                                                sx={{ fontWeight: 600, mb: 1 }}
                                            >
                                                {feedback.comment}
                                            </Typography>
                                            {feedback.rating && (
                                                <Typography
                                                    variant="body2"
                                                    color="text.secondary"
                                                    sx={{ fontWeight: 500, mt: 1 }}
                                                >
                                                    Rating: {feedback.rating}/10
                                                </Typography>
                                            )}
                                            <Box sx={{ display: 'flex', gap: 1, mt: 2 }}>
                                                {(isAdmin || isOrganizer) && (
                                                    <>
                                                        <Tooltip title="Edit Feedback">
                                                            <IconButton
                                                                size="small"
                                                                onClick={() => handleEditFeedbackClick(feedback)}
                                                            >
                                                                <EditIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                        <Tooltip title="Delete Feedback">
                                                            <IconButton
                                                                size="small"
                                                                onClick={() => handleDeleteFeedbackClick(feedback)}
                                                            >
                                                                <DeleteIcon fontSize="small" />
                                                            </IconButton>
                                                        </Tooltip>
                                                    </>
                                                )}
                                            </Box>
                                        </>
                                    )}
                                </CardContent>
                            </Card>
                            </Grid>
                        ))}
                    </Grid>
                ) : (
                    <Typography variant="body1" color="text.secondary">
                        No feedbacks available for this event.
                    </Typography>
                )}
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
            <ConfirmationDialog
                open={showFeedbackDeleteDialog}
                title="Confirm Feedback Deletion"
                message="Are you sure you want to delete this feedback? This action cannot be undone."
                confirmLabel="Delete Feedback"
                cancelLabel="Cancel"
                onConfirm={handleConfirmDeleteFeedback}
                onCancel={() => setShowFeedbackDeleteDialog(false)}
                loading={isFeedbackDeleting}
                isDestructive={true}
            />
        </>
    );
}

export default EventView;