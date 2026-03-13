import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import LoginScreen from '../screens/LoginScreen';
import NewTicketScreen from '../screens/NewTicketScreen';
import WelcomeScreen from '../screens/WelcomeScreen';
import CallListScreen from '../screens/CallListScreen';
import CallDetailsScreen from '../screens/CallDetailsScreen';
const Stack = createStackNavigator();

const AppNavigator = () => {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="Login"
        screenOptions={{
          headerShown: false, // Remove o header padrão para usar nosso design personalizado
        }}
      >
        <Stack.Screen 
          name="Login" 
          component={LoginScreen}
          options={{
            title: 'Login',
          }}
        />
        <Stack.Screen
         name="WelcomeScreen"
         component={WelcomeScreen}
         options={{
           title: 'Hub',
         }}
          />
        <Stack.Screen
         name="CallListScreen"
         component={CallListScreen}
         options={{
           title: 'Lista de Chamados',
         }}
          />  
        <Stack.Screen
         name="CallDetailsScreen"
         component={CallDetailsScreen}
         options={{
           title: 'Descrição dos Chamados',
         }}
          />  
        <Stack.Screen 
          name="NewTicket" 
          component={NewTicketScreen}
          options={{
            title: 'Novo Chamado',
          }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default AppNavigator;
