import React from 'react';
import { Container, Alert, Typography, Button, Box } from '@mui/material';

const ErrorDisplay = ({ 
  error, 
  resetError = null, 
  marginTop = 12, 
  marginBottom = 4,
  fullWidth = false
}) => {
  return (
    <Container maxWidth={fullWidth ? false : "md"} sx={{ mt: marginTop, mb: marginBottom }}>
      <Alert 
        severity="error" 
        sx={{ 
          display: 'flex', 
          alignItems: 'center'
        }}
      >
        <Box sx={{ flexGrow: 1 }}>
          {typeof error === 'string' ? error : 'An error occurred'}
        </Box>
        {resetError && (
          <Button 
            color="error" 
            size="small" 
            onClick={resetError}
          >
            Try Again
          </Button>
        )}
      </Alert>
    </Container>
  );
};

export default ErrorDisplay;