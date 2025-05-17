import React, { useState, useEffect } from 'react';
import {
    Container,
    Typography,
    Box,
    Tabs,
    Tab,
    Paper,
    Button,
    TextField,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    IconButton,
    Divider,
    CircularProgress,
    Alert
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { useAuth } from '../../context/AuthContext';
import { useNotification } from '../../context/NotificationContext';
import { fetchQuestions, createQuestion, updateQuestion, deleteQuestion } from '../../services/questionService';
import EventView from '../events/EventView';

function QuestionsTab() {
    const [questions, setQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [openDialog, setOpenDialog] = useState(false);
    const [dialogType, setDialogType] = useState('add'); // 'add' arba 'edit'
    const [currentQuestion, setCurrentQuestion] = useState({ formulatedQuestion: '', fkAdministratoridUser: null });
    
    const { currentUser } = useAuth();
    const { showSuccess, showError } = useNotification();
    
    // Užkrauti klausimus
    const loadQuestions = async () => {
        try {
            setLoading(true);
            const data = await fetchQuestions();
            setQuestions(data);
            setError(null);
        } catch (err) {
            console.error('Klaida kraunant klausimus:', err);
            setError('Nepavyko gauti klausimų sąrašo. Bandykite vėliau.');
        } finally {
            setLoading(false);
        }
    };
    
    useEffect(() => {
        loadQuestions();
    }, []);
    
    // Dialogo atidarymas - priima 'add' arba 'edit' tipą ir pasirinktą klausimą
    const handleOpenDialog = (type, question = null) => {
        setDialogType(type);
        if (type === 'edit' && question) {
            setCurrentQuestion(question);
        } else {
            setCurrentQuestion({ formulatedQuestion: '', fkAdministratoridUser: currentUser?.id });
        }
        setOpenDialog(true);
    };
    
    // Dialogo uždarymas
    const handleCloseDialog = () => {
        setOpenDialog(false);
    };
    
    // Klausimo teksto keitimas
    const handleQuestionChange = (e) => {
        setCurrentQuestion({
            ...currentQuestion,
            formulatedQuestion: e.target.value
        });
    };
    
    // Klausimo išsaugojimas (sukūrimas arba redagavimas)
    const handleSaveQuestion = async () => {
        if (!currentQuestion.formulatedQuestion.trim()) {
            showError('Klausimo tekstas negali būti tuščias');
            return;
        }
        
        try {
            if (dialogType === 'add') {
                // Sukurti naują klausimą
                await createQuestion({
                    IdQuestion: 0,
                    FormulatedQuestion: currentQuestion.formulatedQuestion,
                    FkAdministratoridUser: currentUser?.id,
                    AdministratorName: currentUser?.name || null
                });
                showSuccess('Klausimas sėkmingai sukurtas!');
            } else {
                // Atnaujinti esamą klausimą
                await updateQuestion({
                    IdQuestion: currentQuestion.idQuestion,
                    FormulatedQuestion: currentQuestion.formulatedQuestion,
                    FkAdministratoridUser: currentUser?.id,
                    AdministratorName: currentUser?.name || null
                });
                showSuccess('Klausimas sėkmingai atnaujintas!');
            }
            
            // Perkrauti klausimų sąrašą
            await loadQuestions();
            handleCloseDialog();
        } catch (error) {
            showError(`Klaida išsaugant klausimą: ${error.message}`);
        }
    };
    
    // Klausimo ištrynimas
    const handleDeleteQuestion = async (questionId) => {
        if (!window.confirm('Ar tikrai norite ištrinti šį klausimą?')) {
            return;
        }
        
        try {
            await deleteQuestion(questionId);
            showSuccess('Klausimas sėkmingai ištrintas!');
            await loadQuestions();
        } catch (error) {
            showError(`Klaida trinant klausimą: ${error.message}`);
        }
    };
    
    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                <CircularProgress />
            </Box>
        );
    }
    
    if (error) {
        return <Alert severity="error" sx={{ mt: 2 }}>{error}</Alert>;
    }
    
    return (
        <Box>
            <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Typography variant="h6" component="h2" sx={{ fontWeight: 600 }}>
                    Klausimų valdymas
                </Typography>
                <Button 
                    variant="contained" 
                    startIcon={<AddIcon />}
                    onClick={() => handleOpenDialog('add')}
                    size="small"
                >
                    Naujas klausimas
                </Button>
            </Box>
            
            <TableContainer component={Paper} sx={{ boxShadow: '0 4px 12px rgba(0,0,0,0.08)', borderRadius: 2 }}>
                <Table size="small">
                    <TableHead>
                        <TableRow sx={{ backgroundColor: 'primary.light' }}>
                            <TableCell sx={{ fontWeight: 'bold', color: 'white' }}>ID</TableCell>
                            <TableCell sx={{ fontWeight: 'bold', color: 'white' }}>Klausimas</TableCell>
                            <TableCell sx={{ fontWeight: 'bold', color: 'white' }}>Administratorius</TableCell>
                            <TableCell sx={{ fontWeight: 'bold', color: 'white' }} align="right">Veiksmai</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {questions.length > 0 ? (
                            questions.map((question) => (
                                <TableRow key={question.idQuestion} hover>
                                    <TableCell>{question.idQuestion}</TableCell>
                                    <TableCell>{question.formulatedQuestion}</TableCell>
                                    <TableCell>{question.administratorName || 'Nenurodyta'}</TableCell>
                                    <TableCell align="right">
                                        <IconButton 
                                            color="primary" 
                                            onClick={() => handleOpenDialog('edit', question)}
                                            size="small"
                                        >
                                            <EditIcon fontSize="small" />
                                        </IconButton>
                                        <IconButton 
                                            color="error" 
                                            onClick={() => handleDeleteQuestion(question.idQuestion)}
                                            size="small"
                                        >
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    </TableCell>
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={4} align="center">
                                    Nėra sukurtų klausimų. Sukurkite naują klausimą.
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </TableContainer>
            
            {/* Klausimo pridėjimo/redagavimo dialogas */}
            <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
                <DialogTitle>
                    {dialogType === 'add' ? 'Pridėti naują klausimą' : 'Redaguoti klausimą'}
                </DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        margin="dense"
                        id="question"
                        label="Klausimo tekstas"
                        type="text"
                        fullWidth
                        multiline
                        rows={3}
                        value={currentQuestion.formulatedQuestion}
                        onChange={handleQuestionChange}
                        variant="outlined"
                        sx={{ mt: 2 }}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseDialog}>Atšaukti</Button>
                    <Button onClick={handleSaveQuestion} variant="contained" color="primary">
                        {dialogType === 'add' ? 'Pridėti' : 'Išsaugoti'}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}

function AdminDashboard() {
    const [tabValue, setTabValue] = useState(0);
    
    const handleTabChange = (event, newValue) => {
        setTabValue(newValue);
    };
    
    return (
        <Container maxWidth="lg" sx={{ mt: 12, mb: 8 }}>
            <Box sx={{ mb: 4 }}>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700, mb: 1 }}>
                    Administravimo skydelis
                </Typography>
                <Typography variant="subtitle1" color="text.secondary" sx={{ mb: 2 }}>
                    Valdykite sistemos nustatymus ir duomenis
                </Typography>
                <Divider />
            </Box>
            
            <Paper sx={{ mb: 3 }}>
                <Tabs 
                    value={tabValue} 
                    onChange={handleTabChange}
                    variant="scrollable"
                    scrollButtons="auto"
                    sx={{ 
                        borderBottom: 1, 
                        borderColor: 'divider',
                        '& .MuiTab-root': { py: 2 }
                    }}
                >
                    <Tab label="Klausimai" />
                    <Tab label="Vartotojai" />
                    <Tab label="Renginiai" />
                </Tabs>
            </Paper>
            
            
            {tabValue === 0 && <QuestionsTab />}
            
            {tabValue === 1 && (
                <Box>
                    <Typography variant="h6" sx={{ mb: 2 }}>Vartotojų valdymas</Typography>
                    
                </Box>
            )}
            
            {tabValue === 2 && (
                <Box>
                    <Typography variant="h6" sx={{ mb: 2 }}>Renginių valdymas</Typography>
                    {EventView}
                </Box>
            )}
        </Container>
    );
}

export default function QuestionManagement() {
    return <AdminDashboard />;
}