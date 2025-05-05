import React from 'react';
import { Box, CircularProgress } from '@mui/material';

const LoadingSpinner = ({ fullHeight = true }) => {
  return (
    <Box 
      display="flex" 
      justifyContent="center" 
      alignItems="center" 
      minHeight={fullHeight ? "100vh" : "200px"}
    >
      <CircularProgress sx={{ color: '#6a11cb' }} />
    </Box>
  );
};

export default LoadingSpinner;