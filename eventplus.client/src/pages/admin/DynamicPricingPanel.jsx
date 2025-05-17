import React, { useState, useEffect } from 'react';
import { 
    Container, 
    Typography, 
    Box, 
    Button,
    Paper,
    Grid,
    TextField,
    CircularProgress,
    Alert,
    Snackbar,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow
} from '@mui/material';
import { dynamicPricingService } from '../../services/dynamicPricingService';
import { fetchEvents } from '../../services/eventService';
import { useAuth } from '../../context/AuthContext';

function DynamicPricingPanel() {
    const [events, setEvents] = useState([]);
    const [selectedEventId, setSelectedEventId] = useState('');
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState(null);
    const [error, setError] = useState(null);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [allEvents, setAllEvents] = useState([]);
    const { currentUser } = useAuth();

    useEffect(() => {
        // Only admin users should access this panel
        if (!currentUser || (currentUser.role !== 'Admin' && currentUser.role !== 'Organiser')) {
            return;
        }

        loadEvents();
    }, [currentUser]);

    const loadEvents = async () => {
        try {
            const fetchedEvents = await fetchEvents();
            
            // Filter for active events (those that haven't ended yet)
            const activeEvents = fetchedEvents.filter(event => {
                return new Date(event.endDate) > new Date();
            });
            
            setAllEvents(fetchedEvents);
            setEvents(activeEvents);
        } catch (err) {
            console.error('Failed to load events:', err);
            setError('Failed to load events');
        }
    };

    const handleEventSelect = (event) => {
        setSelectedEventId(event.target.value);
    };

    const handleAdjustPrice = async () => {
        if (!selectedEventId) {
            setError('Please select an event');
            setSnackbarOpen(true);
            return;
        }

        try {
            setLoading(true);
            setError(null);
            setResult(null);

            const adjustmentResult = await dynamicPricingService.triggerPriceAdjustment(selectedEventId);
            
            setResult(adjustmentResult);
            setSnackbarOpen(true);
        } catch (err) {
            console.error('Error adjusting prices:', err);
            setError(err.message || 'An error occurred');
            setSnackbarOpen(true);
        } finally {
            setLoading(false);
        }
    };

    const handleAdjustAllPrices = async () => {
        try {
            setLoading(true);
            setError(null);
            setResult(null);

            await dynamicPricingService.adjustAllEventPrices();
            
            setResult({ message: 'All event prices have been adjusted' });
            setSnackbarOpen(true);
        } catch (err) {
            console.error('Error adjusting all prices:', err);
            setError(err.message || 'An error occurred');
            setSnackbarOpen(true);
        } finally {
            setLoading(false);
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbarOpen(false);
    };

    // If user is not admin, don't render the component
    if (!currentUser || (currentUser.role !== 'Admin' && currentUser.role !== 'Organiser')) {
        return null;
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 8 }}>
            <Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 700 }}>
                Dynamic Pricing Control Panel
            </Typography>

            <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Adjust Event Pricing
                </Typography>

                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <TextField
                            select
                            fullWidth
                            label="Select Event"
                            value={selectedEventId}
                            onChange={handleEventSelect}
                            SelectProps={{
                                native: true,
                            }}
                            variant="outlined"
                        >
                            <option value="">-- Select Event --</option>
                            {events.map((event) => (
                                <option key={event.idEvent} value={event.idEvent}>
                                    {event.name}
                                </option>
                            ))}
                        </TextField>
                    </Grid>
                    
                    <Grid item xs={12} md={6} sx={{ display: 'flex', alignItems: 'center' }}>
                        <Button 
                            variant="contained" 
                            color="primary"
                            onClick={handleAdjustPrice}
                            disabled={loading || !selectedEventId}
                            sx={{ mr: 2 }}
                        >
                            {loading ? <CircularProgress size={24} /> : 'Adjust Price'}
                        </Button>
                        
                        <Button 
                            variant="outlined" 
                            color="secondary"
                            onClick={handleAdjustAllPrices}
                            disabled={loading}
                        >
                            {loading ? <CircularProgress size={24} /> : 'Adjust All Events'}
                        </Button>
                    </Grid>
                </Grid>

                {result && (
                    <Box sx={{ mt: 3 }}>
                        <Alert severity="success">
                            Price adjustment completed successfully!
                            {result.marketabilityFactor && (
                                <Typography variant="body2" sx={{ mt: 1 }}>
                                    Marketability Factor: {result.marketabilityFactor.toFixed(2)}
                                </Typography>
                            )}
                        </Alert>
                    </Box>
                )}
            </Paper>

            <Paper elevation={3} sx={{ p: 3 }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Event Information
                </Typography>

                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Event Name</TableCell>
                                <TableCell>Category</TableCell>
                                <TableCell>Start Date</TableCell>
                                <TableCell>End Date</TableCell>
                                <TableCell>Max Tickets</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {events.map((event) => (
                                <TableRow key={event.idEvent}>
                                    <TableCell>{event.name}</TableCell>
                                    <TableCell>{event.category}</TableCell>
                                    <TableCell>{new Date(event.startDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{new Date(event.endDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{event.maxTicketCount}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Paper>

            <Snackbar 
                open={snackbarOpen} 
                autoHideDuration={6000} 
                onClose={handleCloseSnackbar}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert 
                    onClose={handleCloseSnackbar} 
                    severity={error ? "error" : "success"} 
                    variant="filled"
                >
                    {error || (result?.message || "Price adjustment completed successfully!")}
                </Alert>
            </Snackbar>
        </Container>
    );
}

export default DynamicPricingPanel;