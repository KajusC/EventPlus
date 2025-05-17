import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
    Container, 
    Typography, 
    Box, 
    Grid, 
    Card, 
    CardContent, 
    Button, 
    Chip,
    IconButton,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle
} from '@mui/material';
import { 
    ConfirmationNumber as TicketIcon, 
    Event as EventIcon, 
    LocationOn as LocationIcon,
    Delete as DeleteIcon,
    Chair as ChairIcon
} from '@mui/icons-material';

import { deleteTicket } from '../../services/ticketService';
import { fetchTicketsByUserId } from '../../services/userTicketService';
import { fetchEventById } from '../../services/eventService';
import LoadingSpinner from '../../components/shared/LoadingSpinner';
import ErrorDisplay from '../../components/shared/ErrorDisplay';
import { useAuth } from '../../context/AuthContext';

import { fetchSeatingById } from '../../services/seatingService'; 
import { fetchSectorById } from '../../services/sectorService'; 
import { fetchTickets } from '../../services/ticketService';
import { fetchOrganiserById } from '../../services/authService';

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


function TicketListPage() {
    const navigate = useNavigate();
    const [tickets, setTickets] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [events, setEvents] = useState({});
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [ticketToDelete, setTicketToDelete] = useState(null);
    const [seatings, setSeatings] = useState({});
    const [sectors, setSectors] = useState({});
    const [event, setEvent] = useState([]);
    

    const { currentUser } = useAuth();
    const [org, setOrg] = useState(null);
    console.log("event", event);
    console.log(tickets);
    useEffect(() => {
        if (!currentUser?.id) return;

        const fetchData = async () => {
            console.log('currentUser:', currentUser);
            const organiser = await fetchOrganiserById(currentUser.id);
            console.log('Organiser:', organiser);
            setOrg(organiser);
        };
        fetchData();
    }, [currentUser]);

    useEffect(() => {
        if (currentUser.role !== 'Admin' && currentUser.role !== 'Organiser') {
            loadUserTickets();
        }
        else {
            loadTickets();
        }
    }, []);

    const loadUserTickets = async () => {
        try {
            setIsLoading(true);
            const userId = currentUser.id;

            if (!userId) {
                navigate('/login');
                return;
            }
            
            const fetchedTickets = await fetchTicketsByUserId(userId);
            if (!fetchedTickets || fetchedTickets.length === 0) {
                setTickets([]);
                setError(null);
                return;
            }
            setTickets(fetchedTickets);
            
            // Load seating and sector information
            const seatingsData = {};
            const sectorsData = {};
            
            await Promise.all(
                fetchedTickets.map(async (ticket) => {
                    if (ticket.fkSeatingidSeating) {
                        try {
                            const seatingData = await fetchSeatingById(ticket.fkSeatingidSeating);
                            seatingsData[ticket.fkSeatingidSeating] = seatingData;
                            
                            if (seatingData && seatingData.sectorId) {
                                const sectorData = await fetchSectorById(seatingData.sectorId);
                                sectorsData[seatingData.sectorId] = sectorData;
                            }
                        } catch (seatingErr) {
                            console.error('Error loading seating data:', seatingErr);
                        }
                    }
                })
            );
            
            setSeatings(seatingsData);
            setSectors(sectorsData);

            // Load event information
            const eventIds = new Set();
            fetchedTickets.forEach(ticket => {
                if (ticket.fkEventidEvent) {
                    eventIds.add(ticket.fkEventidEvent);
                }
            });

            const eventsData = {};
            await Promise.all(
                [...eventIds].map(async (eventId) => {
                    try {
                        const eventData = await fetchEventById(eventId);
                        eventsData[eventId] = eventData;
                    } catch (err) {
                        console.error(`Failed to load event ${eventId}:`, err);
                    }
                })
            );
            
            setEvents(eventsData);
            setError(null);
        } catch (err) {
            if (err.response && err.response.status === 404) {
            // Assume 404 means "no tickets found"
            setTickets([]);
            setError(null);
            } else {
                setError(err.message || 'Failed to load tickets');
            }
        } finally {
            setIsLoading(false);
        }
    };

    const loadTickets = async () => {
        try {
            setIsLoading(true);
            const fetchedTickets = await fetchTickets();
            setTickets(fetchedTickets);
            
            const eventIds = new Set();
            fetchedTickets.forEach(ticket => {
                if (ticket.fkEventidEvent) {
                    eventIds.add(ticket.fkEventidEvent);
                }
            });

            const eventsData = {};
            await Promise.all(
                [...eventIds].map(async (eventId) => {
                    try {
                        const eventData = await fetchEventById(eventId);
                        eventsData[eventId] = eventData;
                    } catch (err) {
                        console.error(`Failed to load event ${eventId}:`, err);
                    }
                })
            );
            
            setEvent(eventsData);
            setError(null);
        } catch (err) {
            setError(err.message || 'Failed to load tickets');
        } finally {
            setIsLoading(false);
        }
    };

    const handleCardClick = (ticketId) => {
        navigate(`/ticket/${ticketId}`);
    };

    const handleDeleteClick = (e, ticketId) => {
        e.stopPropagation(); // Prevent card click event
        setTicketToDelete(ticketId);
        setDeleteDialogOpen(true);
    };

    const handleDeleteConfirm = async () => {
        try {
            await deleteTicket(ticketToDelete);
            setDeleteDialogOpen(false);
            // Refresh ticket list after deletion
            loadUserTickets();
        } catch (err) {
            setError(err.message || 'Failed to delete ticket');
            setDeleteDialogOpen(false);
        }
    };

    const handleDeleteCancel = () => {
        setDeleteDialogOpen(false);
        setTicketToDelete(null);
    };
    
    // Helper function to get seating info display
    const getSeatingInfo = (ticket) => {
        const seating = seatings[ticket.fkSeatingidSeating];
        if (!seating) return 'Not assigned';
        
        const sector = seating.sectorId ? sectors[seating.sectorId] : null;
        const sectorName = sector ? sector.name : 'Unknown sector';
        
        return `${sectorName}, Row ${seating.row}, Seat ${seating.place}`;
    };

    if (isLoading) return <LoadingSpinner />;
    if (error) return <ErrorDisplay error={error} />;

    return (
        <>
            {(currentUser.role === 'Admin' || currentUser.role === 'Organiser') && (
                <Container maxWidth="lg" sx={{ mt: 8, mb: 4 }}>
                    <Box sx={{ 
                        display: 'flex', 
                        justifyContent: 'space-between', 
                        alignItems: 'center', 
                        mb: 4 
                    }}>
                        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
                            My Tickets
                        </Typography>
                    </Box>

                    {tickets.length === 0 ? (
                        <Box 
                            sx={{ 
                                display: 'flex', 
                                flexDirection: 'column', 
                                alignItems: 'center', 
                                justifyContent: 'center', 
                                height: '50vh' 
                            }}
                        >
                            <TicketIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
                            <Typography variant="h6" color="text.secondary">
                                No tickets found
                            </Typography>
                            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                                You haven't purchased any tickets yet
                            </Typography>
                        </Box>
                    ) : (
                        <Grid container spacing={3}>
                            {tickets.map((ticket) => {
                                const ticketType = TICKET_TYPES[ticket.type] || '';
                                const ticketStatus = TICKET_STATUSES[ticket.ticketStatusId] || { label: 'Unknown', color: 'default' };
                                return (
                                    <Grid item xs={12} md={6} key={ticket.idTicket}>
                                        <Card 
                                            sx={{ 
                                                borderRadius: 3, 
                                                overflow: 'hidden', 
                                                transition: 'transform 0.3s ease',
                                                '&:hover': {
                                                    transform: 'scale(1.02)',
                                                    boxShadow: '0 10px 20px rgba(0,0,0,0.1)'
                                                },
                                                position: 'relative'
                                            }}
                                            onClick={() => handleCardClick(ticket.idTicket)}
                                        >
                                            <CardContent sx={{ p: 3 }}>
                                                <Box sx={{ 
                                                    display: 'flex', 
                                                    justifyContent: 'space-between', 
                                                    alignItems: 'center', 
                                                    mb: 2 
                                                }}>
                                                    <Chip 
                                                        label={ticketStatus.label} 
                                                        color={ticketStatus.color}
                                                        size="small"
                                                        sx={{ fontWeight: 600 }}
                                                    />
                                                    <Typography variant="subtitle2" color="text.secondary">
                                                        Ticket #{ticket.idTicket}
                                                    </Typography>
                                                </Box>

                                                <Typography 
                                                    variant="h6" 
                                                    sx={{ 
                                                        fontWeight: 700, 
                                                        mb: 1,
                                                        overflow: 'hidden',
                                                        textOverflow: 'ellipsis',
                                                        whiteSpace: 'nowrap'
                                                    }}
                                                >
                                                    {event[ticket.fkEventidEvent].name} Ticket
                                                </Typography>

                                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                                    <TicketIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Price: {ticket.price ? `$${ticket.price.toFixed(2)}` : 'N/A'}
                                                    </Typography>
                                                </Box>

                                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                                    <EventIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Ticket Type: {ticketType}
                                                    </Typography>
                                                </Box>

                                                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                                    <LocationIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Seating ID: {ticket.seatingId}
                                                    </Typography>
                                                </Box>
                                                
                                                {/* Delete Button */}
                                                <IconButton 
                                                    aria-label="delete ticket"
                                                    onClick={(e) => handleDeleteClick(e, ticket.idTicket)}
                                                    sx={{
                                                        position: 'right',
                                                        top: 8,
                                                        right: 8,
                                                        color: 'error.main',
                                                        '&:hover': {
                                                            backgroundColor: 'rgba(211, 47, 47, 0.04)'
                                                        }
                                                    }}
                                                >
                                                    <DeleteIcon />
                                                </IconButton>
                                            </CardContent>
                                        </Card>
                                    </Grid>
                                );
                            })}
                        </Grid>
                    )}

                    {/* Confirmation Dialog */}
                    <Dialog
                        open={deleteDialogOpen}
                        onClose={handleDeleteCancel}
                        aria-labelledby="alert-dialog-title"
                        aria-describedby="alert-dialog-description"
                    >
                        <DialogTitle id="alert-dialog-title">
                            {"Confirm Ticket Deletion"}
                        </DialogTitle>
                        <DialogContent>
                            <DialogContentText id="alert-dialog-description">
                                Are you sure you want to delete this ticket? This action cannot be undone.
                            </DialogContentText>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleDeleteCancel} color="primary">
                                Cancel
                            </Button>
                            <Button onClick={handleDeleteConfirm} color="error" autoFocus>
                                Delete
                            </Button>
                        </DialogActions>
                    </Dialog>
                </Container>
            )}
            {(currentUser.role !== 'Admin' && currentUser.role !== 'Organiser') && (
                <Container maxWidth="lg" sx={{ mt: 8, mb: 4 }}>
                    <Box sx={{ 
                        display: 'flex', 
                        justifyContent: 'space-between', 
                        alignItems: 'center', 
                        mb: 4 
                    }}>
                        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
                            My Tickets
                        </Typography>
                    </Box>

                    {tickets.length === 0 ? (
                        <Box 
                            sx={{ 
                                display: 'flex', 
                                flexDirection: 'column', 
                                alignItems: 'center', 
                                justifyContent: 'center', 
                                height: '50vh' 
                            }}
                        >
                            <TicketIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
                            <Typography variant="h6" color="text.secondary">
                                No tickets found
                            </Typography>
                            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                                You haven't purchased any tickets yet
                            </Typography>
                        </Box>
                    ) : (
                        <Grid container spacing={3}>
                            {tickets.map((ticket) => {
                                const ticketType = TICKET_TYPES[ticket.type] || '';
                                const ticketStatus = TICKET_STATUSES[ticket.fkTicketstatus] || { label: 'Unknown', color: 'default' };
                                const eventData = events[ticket.fkEventidEvent] || { name: 'Unknown Event' };
                                return (
                                    <Grid item xs={12} md={6} key={ticket.idTicket}>
                                        <Card 
                                            sx={{ 
                                                borderRadius: 3, 
                                                overflow: 'hidden', 
                                                transition: 'transform 0.3s ease',
                                                '&:hover': {
                                                    transform: 'scale(1.02)',
                                                    boxShadow: '0 10px 20px rgba(0,0,0,0.1)'
                                                },
                                                position: 'relative'
                                            }}
                                            onClick={() => handleCardClick(ticket.idTicket)}
                                        >
                                            <CardContent sx={{ p: 3 }}>
                                                <Box sx={{ 
                                                    display: 'flex', 
                                                    justifyContent: 'space-between', 
                                                    alignItems: 'center', 
                                                    mb: 2 
                                                }}>
                                                    <Chip 
                                                        label={ticketStatus.label} 
                                                        color={ticketStatus.color}
                                                        size="small"
                                                        sx={{ fontWeight: 600 }}
                                                    />
                                                    <Typography variant="subtitle2" color="text.secondary">
                                                        Ticket #{ticket.idTicket}
                                                    </Typography>
                                                </Box>

                                                <Typography 
                                                    variant="h6" 
                                                    sx={{ 
                                                        fontWeight: 700, 
                                                        mb: 1,
                                                        overflow: 'hidden',
                                                        textOverflow: 'ellipsis',
                                                        whiteSpace: 'nowrap'
                                                    }}
                                                >
                                                    {eventData.name}
                                                </Typography>

                                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                                    <TicketIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Price: {ticket.price ? `$${ticket.price.toFixed(2)}` : 'N/A'}
                                                    </Typography>
                                                </Box>

                                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                                    <EventIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Ticket Type: {ticketType}
                                                    </Typography>
                                                </Box>

                                                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                                                    <ChairIcon sx={{ 
                                                        color: 'text.secondary', 
                                                        fontSize: 20, 
                                                        mr: 1 
                                                    }} />
                                                    <Typography variant="body2" color="text.secondary">
                                                        Seat: {getSeatingInfo(ticket)}
                                                    </Typography>
                                                </Box>

                                                {(currentUser.role === 'Admin' || currentUser.role === 'Organiser') && (
                                                    <IconButton 
                                                        aria-label="delete ticket"
                                                        onClick={(e) => handleDeleteClick(e, ticket.idTicket)}
                                                        sx={{
                                                            position: 'left',
                                                            top: 8,
                                                            right: 8,
                                                            color: 'error.main',
                                                            '&:hover': {
                                                                backgroundColor: 'rgba(211, 47, 47, 0.04)'
                                                            }
                                                        }}
                                                    >
                                                        <DeleteIcon />
                                                    </IconButton>
                                                )}
                                            </CardContent>
                                        </Card>
                                    </Grid>
                                );
                            })}
                        </Grid>
                    )}

                    {/* Confirmation Dialog */}
                    <Dialog
                        open={deleteDialogOpen}
                        onClose={handleDeleteCancel}
                        aria-labelledby="alert-dialog-title"
                        aria-describedby="alert-dialog-description"
                    >
                        <DialogTitle id="alert-dialog-title">
                            {"Confirm Ticket Deletion"}
                        </DialogTitle>
                        <DialogContent>
                            <DialogContentText id="alert-dialog-description">
                                Are you sure you want to delete this ticket? This action cannot be undone.
                            </DialogContentText>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleDeleteCancel} color="primary">
                                Cancel
                            </Button>
                            <Button onClick={handleDeleteConfirm} color="error" autoFocus>
                                Delete
                            </Button>
                        </DialogActions>
                    </Dialog>
                </Container>
            )}
            
        </>
    );
}

export default TicketListPage;