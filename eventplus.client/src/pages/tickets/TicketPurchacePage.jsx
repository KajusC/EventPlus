import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
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
    Snackbar,
    Paper,
    Stepper,
    Step,
    StepLabel,
    Divider,
    CircularProgress
} from '@mui/material';

import { createTicket, fetchTickets } from '../../services/ticketService';
import { fetchEventById, fetchEvents } from '../../services/eventService';
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';
import { useParams } from 'react-router-dom';
import { fetchSeatings } from '../../services/seatingService';
import { fetchSectors } from '../../services/sectorService';
import { fetchSectorPrices } from '../../services/sectorPriceService';
import { createUserTicket } from '../../services/userTicketService';
import { useAuth } from '../../context/AuthContext';


const TICKET_TYPES = {
    1: 'Standard',
    2: 'VIP',
    3: 'Super-VIP'
};

function TicketPurchasePage() {
    const navigate = useNavigate();
    const { eventId } = useParams();

    const { currentUser } = useAuth();

    const [activeStep, setActiveStep] = useState(0);
    const [events, setEvents] = useState([]);
    const [event, setEvent] = useState(null);
    const [seatingOptions, setSeatingOptions] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);
    const [createdTicketId, setCreatedTicketId] = useState(null);

    const [seatings, setSeatings] = useState([]);
    const [sectorPrices, setSectorPrices] = useState([]);
    const [sectors, setSectors] = useState([]);

    const [existingTickets, setExistingTickets] = useState([]);
    
    const [purchaseData, setPurchaseData] = useState({
        eventId: eventId || '',
        type: 1,
        price: '',
        seatingId: '',
        customerName: '',
        customerEmail: '',
        customerPhone: ''
    });
    
    const [validationErrors, setValidationErrors] = useState({});
    const [snackbar, setSnackbar] = useState({
        open: false,
        message: '',
        severity: 'success'
    });

    // Load event data
    useEffect(() => {
        async function loadEventData() {
            if (!eventId) return;
            
            setIsLoading(true);
            try {
                const ticketData = await fetchTickets();
                setExistingTickets(ticketData);

                const eventData = await fetchEventById(eventId);
                setEvent(eventData);
                
            } catch (err) {
                console.error('Error loading event data:', err);
                setError('Failed to load event data');
            }
        }
        
        loadEventData();
    }, [eventId]);

    // Load seating and sector data
    useEffect(() => {
        async function loadSeatingData() {
            if (!eventId) return;
            
            setIsLoading(true);
            try {
                const [sectorPricesData, sectorsData, seatingsData] = await Promise.all([
                    fetchSectorPrices(),
                    fetchSectors(),
                    fetchSeatings()
                ]);
                console.log('Sector Prices:', sectorPricesData);
                setSectorPrices(sectorPricesData);
                setSectors(sectorsData);
                setSeatings(seatingsData);
                
                // Once data is loaded, set loading to false
                setIsLoading(false);
                
            } catch (err) {
                console.error('Error loading seating data:', err);
                setError('Failed to load seating data');
                setIsLoading(false);
            }
        }
        
        loadSeatingData();
    }, [eventId]);

    useEffect(() => {
        if (!eventId || !sectors.length || !sectorPrices.length || !seatings.length) return;
        
        try {
            const options = generateSeatingOptions(eventId, parseInt(purchaseData.type), sectorPrices, sectors);
            setSeatingOptions(options);

            if (!purchaseData.seatingId) {
                const relevantPrice = sectorPrices.find(
                    sp => sp.eventId === parseInt(eventId) && sp.ticketType === parseInt(purchaseData.type)
                );
                
                if (relevantPrice) {
                    setPurchaseData(prev => ({
                        ...prev,
                        price: relevantPrice.price.toString()
                    }));
                }
            }
        } catch (err) {
            console.error('Error generating seating options:', err);
        }
    }, [eventId, sectors, sectorPrices, seatings, purchaseData.type]);

    
    const generateSeatingOptions = (eventId, ticketType, sectorPricesData, sectorsData) => {
        if (!sectorPricesData || !sectorsData || !eventId || !seatings) {
            return [];
        }
        
        const options = [];
        const eventIdInt = parseInt(eventId);
        
        // Filter prices by event ID and ticket type
        const relevantPrices = sectorPricesData.filter(
            sp => sp.eventId === eventIdInt
        );

        // Track used uniqueKeys to avoid duplicates
        const usedKeys = new Set();

        relevantPrices.forEach(price => {
            const sectorId = price.sectorId;
            const sector = sectorsData.find(s => s.idSector === sectorId);
            if (sector) {
                const sectorSeatings = seatings.filter(seat => seat.sectorId === sectorId);
                
                sectorSeatings.forEach(seating => {
                    if (existingTickets.some(ticket => ticket.seatingId === seating.idSeating))
                        return;
                        
                    // Create a truly unique key by adding price info or a timestamp
                    const uniqueKey = `${seating.idSeating}-${sector.idSector}-${seating.row}-${seating.place}-${price.price}-${Date.now()}`;
                    
                    // Ensure this key hasn't been used already
                    if (!usedKeys.has(uniqueKey)) {
                        usedKeys.add(uniqueKey);
                        options.push({
                            id: seating.idSeating,
                            label: `Section ${sector.name}, Row ${seating.row}, Seat ${seating.place}`,
                            price: parseFloat(price.price.toFixed(2)),
                            uniqueKey: uniqueKey
                        });
                    }
                });
            }
        });
        
        return options;
    };
    const handleChange = (e) => {
        const { name, value } = e.target;
        
        // Special handling for seat selection
        if (name === 'seatingId') {
            const selectedSeat = seatingOptions.find(seat => seat.id === value);
            
            setPurchaseData((prev) => ({
                ...prev,
                [name]: value,
                // Update price when a seat is selected
                price: selectedSeat ? selectedSeat.price.toString() : prev.price
            }));
        } else {
            setPurchaseData((prev) => ({
                ...prev,
                [name]: value
            }));
        }
        
        // Clear validation errors when field is edited
        if (validationErrors[name]) {
            setValidationErrors(prev => ({
                ...prev,
                [name]: null
            }));
        }
        
        // If ticket type changes, reset seating selection
        if (name === 'type') {
            setPurchaseData(prev => ({
                ...prev,
                [name]: value,
                seatingId: '' // Reset seating selection when ticket type changes
            }));
        }
    };

    const validateEventSelection = () => {
        const errors = {};
        
        if (!purchaseData.eventId) {
            errors.eventId = 'Please select an event';
        }
        
        if (!purchaseData.type) {
            errors.type = 'Please select a ticket type';
        }
        
        setValidationErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const validateSeatingAndPrice = () => {
        const errors = {};
        
        if (!purchaseData.seatingId) {
            errors.seatingId = 'Please select a seat';
        }
        
        if (!purchaseData.price) {
            errors.price = 'Price is required';
        } else if (isNaN(purchaseData.price) || Number(purchaseData.price) <= 0) {
            errors.price = 'Price must be a positive number';
        }
        
        setValidationErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const validateCustomerInfo = () => {
        const errors = {};
        
        if (!purchaseData.customerName || purchaseData.customerName.trim() === '') {
            errors.customerName = 'Name is required';
        }
        
        if (!purchaseData.customerEmail || purchaseData.customerEmail.trim() === '') {
            errors.customerEmail = 'Email is required';
        } else if (!/^\S+@\S+\.\S+$/.test(purchaseData.customerEmail)) {
            errors.customerEmail = 'Please enter a valid email address';
        }
        
        if (purchaseData.customerPhone && !/^\+?[0-9\s-()]{6,20}$/.test(purchaseData.customerPhone)) {
            errors.customerPhone = 'Please enter a valid phone number';
        }
        
        setValidationErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleNext = () => {
        let isValid = false;
        
        if (activeStep === 0) {
            isValid = validateEventSelection();
        } else if (activeStep === 1) {
            isValid = validateSeatingAndPrice();
        } else if (activeStep === 2) {
            isValid = validateCustomerInfo();
        }
        
        if (isValid) {
            setActiveStep((prevStep) => prevStep + 1);
        } else {
            setSnackbar({
                open: true,
                message: 'Please fix the validation errors before continuing',
                severity: 'error'
            });
        }
    };

    const handleBack = () => {
        setActiveStep((prevStep) => prevStep - 1);
    };

    const handleSubmit = async () => {
        if (!validateCustomerInfo()) {
            setSnackbar({
                open: true,
                message: 'Please fix the validation errors before submitting',
                severity: 'error'
            });
            return;
        }
        
        try {
            setIsSubmitting(true);
            // Create a new ticket object
            const ticketData = {
                price: parseFloat(purchaseData.price),
                fkEventidEvent: parseInt(purchaseData.eventId),
                type: parseInt(purchaseData.type),
                ticketStatusId: 1, // Active status
                seatingId: parseInt(purchaseData.seatingId), // Convert to integer
                generationDate: new Date().toISOString().split('T')[0],
                qrCode: generateQRCode(purchaseData), // Generate a unique QR code
                scannedDate: null,
            };
            
            console.log('Submitting ticket purchase:', ticketData);

            const result = await createTicket(ticketData);

            const allTickets = await fetchTickets();
            const sortedTickets = allTickets.sort((a, b) => a.idTicket - b.idTicket);
            const lastTicket = sortedTickets[sortedTickets.length - 1];

            await createUserTicket(currentUser.id, lastTicket.idTicket);
            setCreatedTicketId(result.id);
            setSuccess(true);
            
            setSnackbar({
                open: true,
                message: 'Ticket purchased successfully!',
                severity: 'success'
            });
            
            // Move to success step
            setActiveStep(4);
        } catch (err) {
            setError(err.message || 'Failed to purchase ticket');
            setSnackbar({
                open: true,
                message: err.message || 'Failed to purchase ticket',
                severity: 'error'
            });
        } finally {
            setIsSubmitting(false);
        }
    };
    
    const generateQRCode = (data) => {
        const timestamp = new Date().getTime();
        const randomPart = Math.random().toString(36).substring(2, 10);
        return `TIX-${data.eventId}-${data.type}-${timestamp}-${randomPart}`;
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    if (isLoading) return <LoadingSpinner />;
    if (error && !snackbar.open) return <ErrorDisplay error={error} />;

    const getStepContent = (step) => {
        switch (step) {
            case 0:
                return (
                    <EventSelectionStep 
                        purchaseData={purchaseData}
                        handleChange={handleChange}
                        validationErrors={validationErrors}
                        event={event}
                    />
                );
            case 1:
                return (
                    <SeatingAndPriceStep 
                        purchaseData={purchaseData}
                        handleChange={handleChange}
                        validationErrors={validationErrors}
                        seatingOptions={seatingOptions}
                    />
                );
            case 2:
                return (
                    <CustomerInfoStep 
                        purchaseData={purchaseData}
                        handleChange={handleChange}
                        validationErrors={validationErrors}
                    />
                );
            case 3:
                return (
                    <ReviewStep 
                        purchaseData={purchaseData}
                        event={event}
                        seatingOptions={seatingOptions}
                    />
                );
            case 4:
                return (
                    <SuccessStep 
                        navigate={navigate}
                        ticketId={createdTicketId}
                    />
                );
            default:
                return 'Unknown step';
        }
    };

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
            
            {/* Page Header */}
            <Box sx={{ 
                display: 'flex', 
                justifyContent: 'space-between', 
                alignItems: 'center', 
                mb: 4 
            }}>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
                    Purchase Ticket
                </Typography>
            </Box>

            {/* Stepper */}
            <Paper sx={{ 
                mb: 4, 
                p: 3, 
                borderRadius: 3,
                boxShadow: '0 4px 20px rgba(0,0,0,0.08)'
            }}>
                <Stepper activeStep={activeStep} alternativeLabel>
                    <Step>
                        <StepLabel>Select Ticket Type</StepLabel>
                    </Step>
                    <Step>
                        <StepLabel>Choose Seating</StepLabel>
                    </Step>
                    <Step>
                        <StepLabel>Customer Info</StepLabel>
                    </Step>
                    <Step>
                        <StepLabel>Review</StepLabel>
                    </Step>
                    <Step>
                        <StepLabel>Confirmation</StepLabel>
                    </Step>
                </Stepper>
            </Paper>
            
            {/* Step Content */}
            <Card sx={{ 
                borderRadius: 3, 
                overflow: 'hidden', 
                boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
                mb: 4
            }}>
                <CardContent sx={{ p: 4 }}>
                    {getStepContent(activeStep)}
                </CardContent>
            </Card>
            
            {/* Navigation Buttons */}
            {activeStep !== 4 && (
                <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                    {activeStep > 0 && (
                        <Button
                            onClick={handleBack}
                            sx={{ mr: 2 }}
                            disabled={isSubmitting}
                        >
                            Back
                        </Button>
                    )}
                    
                    {activeStep === 3 ? (
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleSubmit}
                            disabled={isSubmitting}
                            sx={{ 
                                borderRadius: 2,
                                fontWeight: 600,
                                py: 1, 
                                px: 3
                            }}
                        >
                            {isSubmitting ? (
                                <CircularProgress size={24} color="inherit" />
                            ) : 'Purchase Ticket'}
                        </Button>
                    ) : (
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleNext}
                            sx={{ 
                                borderRadius: 2,
                                fontWeight: 600,
                                py: 1, 
                                px: 3
                            }}
                        >
                            {activeStep === 2 ? 'Review Order' : 'Next'}
                        </Button>
                    )}
                </Box>
            )}
        </Container>
    );
}

// Step 1: Event Selection Component
function EventSelectionStep({ purchaseData, handleChange, validationErrors, event }) {
    return (
        <>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
                Event: {event && event.name ? event.name : 'Loading event details...'}
            </Typography>
            
            <Grid container spacing={3}>
                <Grid item xs={12}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Ticket Type
                    </Typography>
                    <TextField
                        select
                        name="type"
                        fullWidth
                        value={purchaseData.type}
                        onChange={handleChange}
                        variant="outlined"
                        required
                        sx={{ mt: 1 }}
                        error={!!validationErrors.type}
                        helperText={validationErrors.type}
                    >
                        {Object.entries(TICKET_TYPES).map(([key, label]) => (
                            <MenuItem key={key} value={parseInt(key)}>
                                {label}
                            </MenuItem>
                        ))}
                    </TextField>
                </Grid>
            </Grid>
        </>
    );
}

// Step 2: Seating and Price Component
function SeatingAndPriceStep({ purchaseData, handleChange, validationErrors, seatingOptions }) {
    const selectedSeat = seatingOptions.find(seat => seat.id === parseInt(purchaseData.seatingId));
    
    return (
        <>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
                Select Seating and Confirm Price
            </Typography>
            
            <Grid container spacing={3}>
                <Grid item xs={12}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Seating
                    </Typography>
                    <TextField
                        select
                        name="seatingId"
                        fullWidth
                        value={purchaseData.seatingId}
                        onChange={handleChange}
                        variant="outlined"
                        required
                        sx={{ mt: 1 }}
                        error={!!validationErrors.seatingId}
                        helperText={validationErrors.seatingId || "Select a seat to see its price"}
                        disabled={!seatingOptions || seatingOptions.length === 0}
                    >
                        {seatingOptions && seatingOptions.length > 0 ? (
                            seatingOptions.map((seat) => (
                                <MenuItem key={seat.uniqueKey} value={seat.id}>
                                    {seat.label} - ${seat.price}
                                </MenuItem>
                            ))
                        ) : (
                            <MenuItem disabled>No seating options available</MenuItem>
                        )}
                    </TextField>
                </Grid>
                
                <Grid item xs={12}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Price
                    </Typography>
                    <TextField
                        name="price"
                        type="number"
                        fullWidth
                        value={purchaseData.price}
                        onChange={handleChange}
                        required
                        variant="outlined"
                        sx={{ mt: 1 }}
                        error={!!validationErrors.price}
                        helperText={validationErrors.price}
                        InputProps={{
                            readOnly: true,
                        }}
                    />
                    {selectedSeat && (
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                            This is the price for the selected seat based on the ticket type.
                        </Typography>
                    )}
                    {!selectedSeat && (
                        <Typography variant="body2" color="error" sx={{ mt: 1 }}>
                            Please select a seat to see the price.
                        </Typography>
                    )}
                </Grid>
            </Grid>
        </>
    );
}

// Step 3: Customer Information Component
function CustomerInfoStep({ purchaseData, handleChange, validationErrors }) {
    return (
        <>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
                Enter Customer Information
            </Typography>
            
            <Grid container spacing={3}>
                <Grid item xs={12}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Full Name
                    </Typography>
                    <TextField
                        name="customerName"
                        fullWidth
                        value={purchaseData.customerName || ''}
                        onChange={handleChange}
                        variant="outlined"
                        required
                        sx={{ mt: 1 }}
                        error={!!validationErrors.customerName}
                        helperText={validationErrors.customerName}
                    />
                </Grid>
                
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Email
                    </Typography>
                    <TextField
                        name="customerEmail"
                        type="email"
                        fullWidth
                        value={purchaseData.customerEmail || ''}
                        onChange={handleChange}
                        variant="outlined"
                        required
                        sx={{ mt: 1 }}
                        error={!!validationErrors.customerEmail}
                        helperText={validationErrors.customerEmail}
                    />
                </Grid>
                
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Phone Number
                    </Typography>
                    <TextField
                        name="customerPhone"
                        fullWidth
                        value={purchaseData.customerPhone || ''}
                        onChange={handleChange}
                        variant="outlined"
                        sx={{ mt: 1 }}
                        error={!!validationErrors.customerPhone}
                        helperText={validationErrors.customerPhone}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                       Card Number
                    </Typography>
                    <TextField
                        name="customerCard"
                        fullWidth
                        variant="outlined"
                        sx={{ mt: 1 }}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Expiration Date
                    </Typography>
                    <TextField
                        name="customerExpiration"
                        type="date"
                        fullWidth
                        onChange={handleChange}
                        variant="outlined"
                        sx={{ mt: 1 }}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        CVV
                    </Typography>
                    <TextField
                        name="customerCVV"
                        fullWidth
                        onChange={handleChange}
                        variant="outlined"
                        sx={{ mt: 1 }}
                    />
                </Grid>
            </Grid>
        </>
    );
}

// Step 4: Review Component
function ReviewStep({ purchaseData, event, seatingOptions }) {
    const selectedSeat = seatingOptions?.find(s => s.id === purchaseData.seatingId) || {};
    return (
        <>
            <Typography variant="h6" sx={{ fontWeight: 700, mb: 3 }}>
                Review Your Order
            </Typography>
            
            <Box sx={{ mb: 4 }}>
                <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
                    Event Details
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, borderRadius: 2 }}>
                    <Typography variant="body1">
                        <strong>Event:</strong> {event?.name || 'N/A'}
                    </Typography>
                    <Typography variant="body1">
                        <strong>Date:</strong> {event?.startDate || 'N/A'}
                    </Typography>
                    <Typography variant="body1">
                        <strong>Ticket Type:</strong> {TICKET_TYPES[purchaseData.type] || 'N/A'}
                    </Typography>
                    <Typography variant="body1">
                        <strong>Seating:</strong> {selectedSeat?.label || 'N/A'}
                    </Typography>
                </Paper>
            </Box>
            
            <Box sx={{ mb: 4 }}>
                <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
                    Customer Information
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, borderRadius: 2 }}>
                    <Typography variant="body1">
                        <strong>Name:</strong> {purchaseData.customerName || 'N/A'}
                    </Typography>
                    <Typography variant="body1">
                        <strong>Email:</strong> {purchaseData.customerEmail || 'N/A'}
                    </Typography>
                    {purchaseData.customerPhone && (
                        <Typography variant="body1">
                            <strong>Phone:</strong> {purchaseData.customerPhone}
                        </Typography>
                    )}
                </Paper>
            </Box>
            
            <Box>
                <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
                    Payment Summary
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, borderRadius: 2 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                        <Typography variant="body1">Ticket Price:</Typography>
                        <Typography variant="body1">${parseFloat(purchaseData.price || 0).toFixed(2)}</Typography>
                    </Box>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                        <Typography variant="body1">Service Fee:</Typography>
                        <Typography variant="body1">${(parseFloat(purchaseData.price || 0) * 0.10).toFixed(2)}</Typography>
                    </Box>
                    <Divider sx={{ my: 2 }} />
                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Typography variant="h6" sx={{ fontWeight: 600 }}>Total:</Typography>
                        <Typography variant="h6" sx={{ fontWeight: 600 }}>
                            ${(parseFloat(purchaseData.price || 0) * 1.10).toFixed(2)}
                        </Typography>
                    </Box>
                </Paper>
                    <Button
                        variant="contained"
                        color="primary"
                        sx={{ 
                            borderRadius: 2,
                            fontWeight: 600,
                            py: 1.5, 
                            px: 4
                        }}
                    >
                        Use Loyalty Points
                    </Button>
            </Box>
        </>
    );
}

// Step 5: Success Component
function SuccessStep({ navigate, ticketId }) {
    return (
        <Box sx={{ textAlign: 'center', py: 4 }}>
            <Typography variant="h5" sx={{ fontWeight: 700, color: 'success.main', mb: 3 }}>
                Purchase Successful!
            </Typography>
            
            <Paper 
                variant="outlined" 
                sx={{ 
                    p: 3, 
                    borderRadius: 3, 
                    maxWidth: 400, 
                    mx: 'auto',
                    borderColor: 'success.light',
                    bgcolor: 'success.light',
                    color: 'success.contrastText',
                    mb: 4
                }}
            >
                <Typography variant="body1" sx={{ mb: 2 }}>
                    Your ticket has been successfully purchased and is now active.
                    {ticketId && <span> Ticket ID: {ticketId}</span>}
                </Typography>
            </Paper>
            
            <Button
                variant="contained"
                color="primary"
                onClick={() => navigate('/tickets')} 
                sx={{ 
                    borderRadius: 2,
                    fontWeight: 600,
                    py: 1.5, 
                    px: 4
                }}
            >
                View Your Tickets
            </Button>
        </Box>
    );
}

export default TicketPurchasePage;