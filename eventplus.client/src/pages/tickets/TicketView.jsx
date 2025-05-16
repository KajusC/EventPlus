import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
    Container, 
    Typography, 
    Box, 
    Grid, 
    Card, 
    CardContent, 
    Chip, 
    Button, 
    IconButton, 
    Divider 
} from '@mui/material';
import { 
    ConfirmationNumber as TicketIcon,
    Event as EventIcon,
    LocationOn as LocationIcon,
    QrCode as QrCodeIcon,
    ArrowBack as ArrowBackIcon,
    DeleteOutline as DeleteIcon,
    Edit as EditIcon
} from '@mui/icons-material';

import { fetchTicketById } from '../../services/ticketService';
import { formatDate, formatTime } from '../../utils/dateFormatter';
import { fetchSeatingById } from '../../services/seatingService'; 
import { fetchSectorById } from '../../services/sectorService'; 

import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';
import ConfirmationDialog from '../../components/shared/ConfirmationDialog';
import ToastNotification from '../../components/shared/ToastNotification';
import { fetchEventById } from '../../services/eventService';
import { useAuth } from '../../context/AuthContext';

// Ticket type mapping
const TICKET_TYPES = {
    1: 'Standard',
    2: 'VIP',
    3: 'Super-VIP'
};

// Ticket status mapping
const TICKET_STATUSES = {
    1: { label: 'Active', color: 'success' },
    2: { label: 'Inactive', color: 'default' },
    3: { label: 'Scanned', color: 'warning' }
};

function TicketView() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { currentUser } = useAuth();
    const [ticket, setTicket] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [event, setEvent] = useState(null);
    const [seating, setSeating] = useState(null);
    const [sector, setSector] = useState(null);
    const [toast, setToast] = useState({
        open: false,
        message: '',
        severity: 'info'
    });

    useEffect(() => {
        const loadTicket = async () => {
            try {
                setIsLoading(true);
                const fetchedTicket = await fetchTicketById(id);
                setTicket(fetchedTicket);

                const eventData = await fetchEventById(fetchedTicket.fkEventidEvent);
                setEvent(eventData);
                
                // Fetch seating information if seatingId exists
                if (fetchedTicket.seatingId) {
                    try {
                        const seatingData = await fetchSeatingById(fetchedTicket.seatingId);
                        setSeating(seatingData);
                        
                        // Fetch sector information
                        if (seatingData.sectorId) {
                            const sectorData = await fetchSectorById(seatingData.sectorId);
                            setSector(sectorData);
                        }
                    } catch (seatingErr) {
                        console.error('Error loading seating data:', seatingErr);
                        // Don't fail the entire component if seating info can't be fetched
                    }
                }
                
                setError(null);
            } catch (err) {
                setError(err.message || 'Failed to load ticket');
            } finally {
                setIsLoading(false);
            }
        };

        loadTicket();
    }, [id]);

    // Helper function to check if date is default/uninitialized value
    const isDefaultDate = (dateString) => {
        if (!dateString) return true;
        
        // Check for default SQL date or .NET default date
        return dateString === '0001-01-01T00:00:00' || 
               dateString === '1900-01-01T00:00:00' || 
               new Date(dateString).getFullYear() <= 1901;
    };

    if (isLoading) return <LoadingSpinner />;
    if (error) return <ErrorDisplay error={error} />;
    if (!ticket) return <ErrorDisplay error="Ticket not found" />;

    // Destructure ticket with fallback values
    const {
        idTicket,
        price,
        generationDate,
        scannedDate,
        qrCode,
        type,
        fkEventidEvent,
        seatingId,
        ticketStatusId
    } = ticket;

    // Get ticket type and status
    const ticketType = TICKET_TYPES[type] || 'Standard';
    const ticketStatus = TICKET_STATUSES[ticketStatusId] || { label: 'Unknown', color: 'default' };
    
    // Get formatted seating information
    const formattedSeating = seating && sector 
        ? `Section ${sector.name}, Row ${seating.row}, Seat ${seating.place}`
        : seatingId 
            ? `Seating ID: ${seatingId}` 
            : 'General Admission';

    return (
        <Container maxWidth="lg" sx={{ mt: 8, mb: 4 }}>
            {/* Ticket Header */}
            <Box sx={{ 
                display: 'flex', 
                justifyContent: 'space-between', 
                alignItems: 'center', 
                mb: 4 
            }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <IconButton 
                        onClick={() => navigate('/tickets')} 
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
                        Ticket Details
                    </Typography>
                </Box>
                {(currentUser.role === 'Admin' || currentUser.role === 'Organiser') && (
                    <Box sx={{ display: 'flex', gap: 2 }}>
                        <IconButton
                            onClick={() => navigate(`/ticket/edit/${id}`)}
                            color="primary"
                        >
                            <EditIcon />
                        </IconButton>
                    </Box>
                )}
            </Box>

            <Grid container spacing={4}>
                {/* Ticket Main Details */}
                <Grid item xs={12} md={8}>
                    <Card sx={{ 
                        borderRadius: 3, 
                        overflow: 'hidden', 
                        boxShadow: '0 10px 30px rgba(0,0,0,0.1)' 
                    }}>
                        <CardContent sx={{ p: 4 }}>
                            {/* Ticket Status and ID */}
                            <Box sx={{ 
                                display: 'flex', 
                                justifyContent: 'space-between', 
                                alignItems: 'center', 
                                mb: 3 
                            }}>
                                <Chip 
                                    label={ticketStatus.label} 
                                    color={ticketStatus.color}
                                    sx={{ fontWeight: 600 }}
                                />
                                <Typography variant="subtitle1" color="text.secondary">
                                    Ticket #{idTicket}
                                </Typography>
                            </Box>

                            {/* Ticket Details */}
                            <Box>
                                <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
                                    Ticket Information
                                </Typography>
                                <Grid container spacing={3}>
                                    {/* Ticket Type */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Ticket Type
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {ticketType}
                                        </Typography>
                                    </Grid>

                                    {/* Price */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Price
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {price ? `$${price.toFixed(2)}` : 'N/A'}
                                        </Typography>
                                    </Grid>

                                    {/* Generation Date */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Generation Date
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {generationDate ? formatDate(generationDate) : 'N/A'}
                                        </Typography>
                                    </Grid>

                                    {/* Scanned Date */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Scanned Date
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {!isDefaultDate(scannedDate) ? formatDate(scannedDate) : 'Not Scanned'}
                                        </Typography>
                                    </Grid>

                                    {/* Event ID */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Event
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {event?.name || 'N/A'}
                                        </Typography>
                                    </Grid>

                                    {/* Seating Information */}
                                    <Grid item xs={12} sm={6}>
                                        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                            Seating
                                        </Typography>
                                        <Typography variant="body1" fontWeight={500}>
                                            {formattedSeating}
                                        </Typography>
                                    </Grid>
                                </Grid>
                            </Box>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>

            {/* Toast notifications */}
            <ToastNotification
                open={toast.open}
                message={toast.message}
                severity={toast.severity}
                onClose={() => setToast({...toast, open: false})}
            />
        </Container>
    );
}

export default TicketView;