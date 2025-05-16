import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    Container,
    Typography,
    TextField,
    Button,
    MenuItem,
    Grid,
    Box,
    Card,
    CardContent,
    Alert,
    Snackbar
} from '@mui/material';

import { fetchTicketById, updateTicket } from '../../services/ticketService';
import { fetchEvents } from '../../services/eventService';
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';

const TICKET_TYPES = {
    1: 'Standard',
    2: 'VIP',
    3: 'Super-VIP'
};

const TICKET_STATUSES = {
    1: 'Active',
    2: 'Inactive',
    3: 'Scanned'
};

function TicketEditPage() {
    const { id } = useParams();
    const navigate = useNavigate();

    const [ticket, setTicket] = useState(null);
    const [events, setEvents] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [validationErrors, setValidationErrors] = useState({});
    const [snackbar, setSnackbar] = useState({
        open: false,
        message: '',
        severity: 'success'
    });

    useEffect(() => {
        const loadData = async () => {
            try {
                setIsLoading(true);
                const ticketData = await fetchTicketById(id);
                const eventList = await fetchEvents();

                setTicket(ticketData);
                setEvents(eventList);
            } catch (err) {
                setError(err.message || 'Klaida kraunant duomenis');
            } finally {
                setIsLoading(false);
            }
        };
        loadData();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setTicket((prev) => ({
            ...prev,
            [name]: value
        }));
        
        // Clear validation error when field is edited
        if (validationErrors[name]) {
            setValidationErrors(prev => ({
                ...prev,
                [name]: null
            }));
        }
    };

    const validateForm = () => {
        const errors = {};
        
        // Price validation
        if (!ticket.price) {
            errors.price = 'Price is required';
        } else if (isNaN(ticket.price) || Number(ticket.price) <= 0) {
            errors.price = 'Price must be a positive number';
        }
        
        // QR Code validation
        if (!ticket.qrCode || ticket.qrCode.trim() === '') {
            errors.qrCode = 'QR Code is required';
        }
        
        // Generation Date validation
        if (!ticket.generationDate) {
            errors.generationDate = 'Generation date is required';
        }
        
        // Event validation
        if (!ticket.fkEventidEvent) {
            errors.fkEventidEvent = 'Event selection is required';
        }
        
        // Validate scan date is after generation date
        if (ticket.scannedDate && ticket.generationDate) {
            const scanDate = new Date(ticket.scannedDate);
            const genDate = new Date(ticket.generationDate);
            
        }
        
        // Type validation
        if (!ticket.type) {
            errors.type = 'Ticket type is required';
        }
        
        // Status validation
        if (!ticket.ticketStatusId) {
            errors.ticketStatusId = 'Ticket status is required';
        }

        setValidationErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!validateForm()) {
            setSnackbar({
                open: true,
                message: 'Please fix validation errors before submitting',
                severity: 'error'
            });
            return;
        }
        
        try {
            console.log('Submitting ticket:', ticket);
            await updateTicket(ticket);
            setSnackbar({
                open: true,
                message: 'Ticket updated successfully',
                severity: 'success'
            });
            setTimeout(() => {
                navigate(`/ticket/${ticket.idTicket}`);
            }, 1500);
        } catch (err) {
            setError(err.message || 'Nepavyko išsaugoti bilieto');
            setSnackbar({
                open: true,
                message: err.message || 'Failed to save ticket',
                severity: 'error'
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    if (isLoading) return <LoadingSpinner />;
    if (error) return <ErrorDisplay error={error} />;
    if (!ticket) return <ErrorDisplay error="Bilietas nerastas" />;

    return (
        <Container maxWidth="lg" sx={{ mt: 8, mb: 4 }}>
            {/* Snackbar for notifications */}
            <Snackbar 
                open={snackbar.open} 
                autoHideDuration={6000} 
                onClose={handleCloseSnackbar}
                anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
            >
                <Alert 
                    onClose={handleCloseSnackbar} 
                    severity={snackbar.severity} 
                    variant="filled"
                    sx={{ width: '100%' }}
                >
                    {snackbar.message}
                </Alert>
            </Snackbar>
            
            {/* Ticket Header */}
            <Box sx={{ 
                display: 'flex', 
                justifyContent: 'space-between', 
                alignItems: 'center', 
                mb: 4 
            }}>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
                    Edit Ticket
                </Typography>
            </Box>

            <form onSubmit={handleSubmit}>
                <Card sx={{ 
                    borderRadius: 3, 
                    overflow: 'hidden', 
                    boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                    mb: 4
                }}>
                    <CardContent sx={{ p: 4 }}>
                        <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
                            Ticket Info
                        </Typography>
                        
                        <Grid container spacing={3}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Price
                                </Typography>
                                <TextField
                                    name="price"
                                    type="number"
                                    fullWidth
                                    value={ticket.price}
                                    onChange={handleChange}
                                    required
                                    variant="outlined"
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.price}
                                    helperText={validationErrors.price}
                                    inputProps={{ min: "0.01", step: "0.01" }}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    QR Code
                                </Typography>
                                <TextField
                                    name="qrCode"
                                    fullWidth
                                    value={ticket.qrCode}
                                    onChange={handleChange}
                                    variant="outlined"
                                    required
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.qrCode}
                                    helperText={validationErrors.qrCode}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Generation Date
                                </Typography>
                                <TextField
                                    name="generationDate"
                                    type="date"
                                    fullWidth
                                    value={ticket.generationDate?.split('T')[0] || ''}
                                    onChange={handleChange}
                                    variant="outlined"
                                    required
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.generationDate}
                                    helperText={validationErrors.generationDate}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Scan Date
                                </Typography>
                                <TextField
                                    name="scannedDate"
                                    type="datetime-local"
                                    fullWidth
                                    value={ticket.scannedDate ? ticket.scannedDate.slice(0, 16) : ''}
                                    onChange={handleChange}
                                    InputLabelProps={{ shrink: true }}
                                    variant="outlined"
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.scannedDate}
                                    helperText={validationErrors.scannedDate}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Ticket Type
                                </Typography>
                                <TextField
                                    select
                                    name="type"
                                    fullWidth
                                    value={ticket.type}
                                    onChange={handleChange}
                                    variant="outlined"
                                    required
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.type}
                                    helperText={validationErrors.type}
                                >
                                    {Object.entries(TICKET_TYPES).map(([key, label]) => (
                                        <MenuItem key={key} value={key}>
                                            {label}
                                        </MenuItem>
                                    ))}
                                </TextField>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Ticket Status
                                </Typography>
                                <TextField
                                    select
                                    name="ticketStatusId"
                                    fullWidth
                                    value={ticket.ticketStatusId}
                                    onChange={handleChange}
                                    variant="outlined"
                                    required
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.ticketStatusId}
                                    helperText={validationErrors.ticketStatusId}
                                >
                                    {Object.entries(TICKET_STATUSES).map(([key, label]) => (
                                        <MenuItem key={key} value={key}>
                                            {label}
                                        </MenuItem>
                                    ))}
                                </TextField>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Event
                                </Typography>
                                <TextField
                                    select
                                    name="fkEventidEvent"
                                    fullWidth
                                    value={ticket.fkEventidEvent}
                                    onChange={handleChange}
                                    variant="outlined"
                                    required
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.fkEventidEvent}
                                    helperText={validationErrors.fkEventidEvent}
                                >
                                    {events.map((event) => (
                                        <MenuItem key={event.idEvent} value={event.idEvent}>
                                            {event.name}
                                        </MenuItem>
                                    ))}
                                </TextField>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Seating ID
                                </Typography>
                                <TextField
                                    name="seatingId"
                                    fullWidth
                                    value={ticket.seatingId}
                                    onChange={handleChange}
                                    variant="outlined"
                                    sx={{ mt: 1 }}
                                    error={!!validationErrors.seatingId}
                                    helperText={validationErrors.seatingId}
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>
                
                <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                    <Button
                        type="submit"
                        variant="contained"
                        color="primary"
                        sx={{ 
                            borderRadius: 2,
                            fontWeight: 600,
                            py: 1, 
                            px: 3
                        }}
                    >
                        Save Changes
                    </Button>
                </Box>
            </form>
        </Container>
    );
}

export default TicketEditPage;