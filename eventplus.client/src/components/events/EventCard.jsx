import React from 'react';
import { Card, CardContent, Typography, Button, CardActions, Box, Chip, useTheme, CardMedia } from '@mui/material';
import { styled } from '@mui/system';
import { NavLink } from 'react-router-dom';
import { CalendarMonth, LocationOn, ConfirmationNumber } from '@mui/icons-material';

const StyledCard = styled(Card)(({ theme }) => ({
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-between',
    borderRadius: theme.spacing(2),
    boxShadow: '0px 10px 30px rgba(0, 0, 0, 0.1)',
    overflow: 'hidden',
    transition: 'all 0.3s ease-in-out',
    position: 'relative',
    '&:hover': {
        transform: 'translateY(-8px)',
        boxShadow: '0px 20px 40px rgba(0, 0, 0, 0.2)',
    },
}));

const CardImageOverlay = styled(Box)(({ theme }) => ({
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: '35%',
    background: 'linear-gradient(180deg, rgba(0,0,0,0.7) 0%, rgba(0,0,0,0) 100%)',
    zIndex: 1,
}));

const CardTopContent = styled(Box)(({ theme }) => ({
    position: 'relative',
    zIndex: 2,
    padding: theme.spacing(2),
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
}));

const StyledCardContent = styled(CardContent)(({ theme }) => ({
    padding: theme.spacing(3),
    paddingTop: theme.spacing(1),
    flexGrow: 1,
}));

const StyledCardActions = styled(CardActions)(({ theme }) => ({
    padding: theme.spacing(2, 3, 3),
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderTop: '1px solid rgba(0,0,0,0.05)',
}));

const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    const date = new Date(dateString);
    return date.toLocaleString(undefined, options);
};

const formatTime = (dateString) => {
    const options = { hour: 'numeric', minute: 'numeric' };
    const date = new Date(dateString);
    return date.toLocaleString(undefined, options);
};

// Event category to image mapping
const getCategoryImage = (category) => {
    const images = {
        1: 'https://source.unsplash.com/random/400x200/?concert',
        2: 'https://source.unsplash.com/random/400x200/?conference',
        // Add more mappings as needed
        default: 'https://source.unsplash.com/random/400x200/?event'
    };
    return images[category] || images.default;
};

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

function EventCard({ event }) {
    const theme = useTheme();
    const categoryName = getCategoryName(event.category);

    return (
        <StyledCard>
            <Box sx={{ position: 'relative' }}>
                <CardImageOverlay />
                <CardTopContent>
                    <Chip 
                        label={categoryName}
                        color="secondary"
                        size="small"
                        sx={{ 
                            fontWeight: 600,
                            backgroundColor: 'rgba(106, 17, 203, 0.8)',
                            color: 'white',
                            backdropFilter: 'blur(4px)',
                            '& .MuiChip-label': {
                                px: 1.5,
                            }
                        }}
                    />
                </CardTopContent>
            </Box>

            <StyledCardContent>
                <Typography 
                    variant="h6" 
                    component="h2" 
                    gutterBottom
                    sx={{ 
                        fontWeight: 700,
                        fontSize: '1.1rem',
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        height: '2.8rem',
                        mb: 1.5
                    }}
                >
                    {event.name}
                </Typography>

                <Typography 
                    variant="body2" 
                    color="text.secondary"
                    sx={{
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                        display: '-webkit-box',
                        WebkitLineClamp: 3,
                        WebkitBoxOrient: 'vertical',
                        height: '4.5rem',
                        mb: 2
                    }}
                >
                    {event.description}
                </Typography>

                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                    <CalendarMonth sx={{ color: 'text.secondary', fontSize: 20, mr: 1 }} />
                    <Typography variant="body2" color="text.primary" fontWeight={500}>
                        {formatDate(event.startDate)} · {formatTime(event.startDate)}
                    </Typography>
                </Box>

                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <ConfirmationNumber sx={{ color: 'text.secondary', fontSize: 20, mr: 1 }} />
                    <Typography variant="body2" color="text.primary" fontWeight={500}>
                        {event.maxTicketCount} tickets available
                    </Typography>
                </Box>
            </StyledCardContent>

            <StyledCardActions>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <Typography 
                        variant="subtitle2" 
                        color="primary"
                        sx={{ 
                            fontWeight: 600,
                            background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                            WebkitBackgroundClip: 'text',
                            WebkitTextFillColor: 'transparent',
                        }}
                    >
                        View Details
                    </Typography>
                </Box>
                <Button
                    variant="contained"
                    component={NavLink}
                    to={`/eventview/${event.idEvent}`}
                    size="small"
                    sx={{
                        fontWeight: 600,
                        borderRadius: 2,
                        px: 2.5,
                        py: 0.8,
                        textTransform: 'none',
                        background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                        boxShadow: '0 3px 5px 2px rgba(106, 17, 203, .3)',
                        '&:hover': {
                            boxShadow: '0 5px 8px 2px rgba(106, 17, 203, .35)',
                        }
                    }}
                >
                    Book Now
                </Button>
            </StyledCardActions>
        </StyledCard>
    );
}

export default EventCard;