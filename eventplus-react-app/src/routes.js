import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import MainMenu from './pages/MainMenu';
import EventView from './pages/events/EventView';
import EventList from './pages/events/EventList';
import EventInsert from './pages/events/EventInsert';
import EventEdit from './pages/events/EventEdit';

const Routes = () => {
    return (
        <Router>
            <Switch>
                <Route path="/" exact component={MainMenu} />
                <Route path="/events" exact component={EventList} />
                <Route path="/eventview/:id" component={EventView} />
                <Route path="/eventinsert" component={EventInsert} />
                <Route path="/eventedit/:id" component={EventEdit} />
            </Switch>
        </Router>
    );
};

export default Routes;