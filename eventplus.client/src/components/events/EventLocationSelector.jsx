import React, { useState, useEffect } from 'react';
import { 
  Box, 
  Typography, 
  Card, 
  CardContent, 
  Grid, 
  Rating, 
  Button, 
  Divider,
  FormControl,
  FormHelperText
} from '@mui/material';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import { getEventLocations, getFeedbackByLocation, getLocationRatings } from '../../services/eventLocationService';
import LoadingSpinner from '../shared/LoadingSpinner';

const EventLocationSelector = ({ onLocationSelect, selectedLocationId, error }) => {
  const [locations, setLocations] = useState([]);
  const [locationFeedback, setLocationFeedback] = useState({});
  const [locationRatings, setLocationRatings] = useState({});
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState(null);

  useEffect(() => {
    const loadEventLocations = async () => {
      try {
        setLoading(true);
        const data = await getEventLocations();
        setLocations(data);
        
        // Load feedback and ratings for each location
        const feedbackPromises = data.map(loc => getFeedbackByLocation(loc.idEventLocation));
        const ratingsPromises = data.map(loc => getLocationRatings(loc.idEventLocation));
        
        const feedbackResults = await Promise.all(feedbackPromises);
        const ratingsResults = await Promise.all(ratingsPromises);
        
        const feedbackMap = {};
        const ratingsMap = {};
        
        data.forEach((location, index) => {
          feedbackMap[location.idEventLocation] = feedbackResults[index];
          ratingsMap[location.idEventLocation] = ratingsResults[index];
        });
        
        setLocationFeedback(feedbackMap);
        setLocationRatings(ratingsMap);
        setLoadError(null);
      } catch (err) {
        console.error("Failed to load event locations:", err);
        setLoadError("Failed to load event locations. Please try again later.");
      } finally {
        setLoading(false);
      }
    };
    
    loadEventLocations();
  }, []);
  
  const getAverageRating = (locationId) => {
    const ratings = locationRatings[locationId];
    if (!ratings || ratings.length === 0) return 0;
    
    const sum = ratings.reduce((total, current) => total + current.rating, 0);
    return sum / ratings.length;
  };
  
  const getRecentFeedback = (locationId, count = 2) => {
    const feedback = locationFeedback[locationId];
    if (!feedback || feedback.length === 0) return [];
    
    return feedback.slice(0, count);
  };
  
  if (loading) return <LoadingSpinner fullHeight={false} />;
  if (loadError) return <Typography color="error">{loadError}</Typography>;
  
  return (
    <FormControl fullWidth error={!!error}>
      <Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
        Select Event Location
      </Typography>
      
      <Grid container spacing={3}>
        {locations.map((location) => (
          <Grid item xs={12} md={6} key={location.idEventLocation}>
            <Card 
              sx={{ 
                cursor: 'pointer',
                border: selectedLocationId === location.idEventLocation ? '2px solid #6a11cb' : '1px solid rgba(0,0,0,0.12)',
                '&:hover': { boxShadow: '0 4px 12px rgba(0,0,0,0.15)' }
              }}
              onClick={() => onLocationSelect(location.idEventLocation)}
            >
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Typography variant="h6" gutterBottom>
                    {location.name}
                  </Typography>
                  
                  <Rating 
                    value={getAverageRating(location.idEventLocation)} 
                    readOnly 
                    precision={0.5}
                    size="small"
                  />
                </Box>
                
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  <LocationOnIcon sx={{ fontSize: 18, mr: 0.5, color: 'text.secondary' }} />
                  <Typography variant="body2" color="text.secondary">
                    {location.address}
                  </Typography>
                </Box>
                
                <Typography variant="body2" sx={{ mb: 2 }}>
                  {location.description}
                </Typography>
                
                <Box sx={{ mb: 1 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Capacity: {location.capacity} attendees
                  </Typography>
                  <Typography variant="subtitle2">
                    Equipment: {location.hasEquipment ? 'Available' : 'Not available'}
                  </Typography>
                </Box>
                
                {getRecentFeedback(location.idEventLocation).length > 0 && (
                  <>
                    <Divider sx={{ my: 1.5 }} />
                    <Typography variant="subtitle2" gutterBottom>
                      Recent Feedback:
                    </Typography>
                    {getRecentFeedback(location.idEventLocation).map((feedback, index) => (
                      <Typography key={index} variant="body2" sx={{ mb: 0.5, fontStyle: 'italic' }}>
                        "{feedback.comment}"
                      </Typography>
                    ))}
                  </>
                )}
                
                <Button 
                  variant="contained" 
                  size="small" 
                  sx={{ mt: 2, backgroundImage: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)' }}
                  onClick={() => onLocationSelect(location.idEventLocation)}
                >
                  Select This Location
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      
      {error && (
        <FormHelperText error>{error}</FormHelperText>
      )}
    </FormControl>
  );
};

export default EventLocationSelector;